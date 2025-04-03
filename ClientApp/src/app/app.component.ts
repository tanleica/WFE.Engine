import { Component, AfterViewInit, inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ISaga } from '../models/saga.model';
import { Subscription } from 'rxjs';
import { WorkflowAdminService } from './workflow-admin.service';
import { VietnameseDatetimePipe } from './vietnamese-datetime.pipe';

enum EnumStepType {
  SINGLE = 'single',
  SHORT_CIRCUIT = 'short_circuit',
  PARALLEL = 'parallel',
}

@Component({
  selector: 'app-root',
  imports: [ VietnameseDatetimePipe ],
  templateUrl: './app.component.html'
})
export class AppComponent implements AfterViewInit, OnDestroy {

  saga: ISaga | null = null;
  leftBarShown: boolean = false;
  subscriptions: Subscription[] = [];
  lastCompletedStepOrder: number = -1;
  stepType: EnumStepType = EnumStepType.SINGLE;
  stepTypeReactedCount: number = 0;

  actors = [
    {
      id: '89bb28ef-f39b-48db-a7b5-b076d030bcfa',
      name: 'Me',
      pushSubscriptionDomain: 'https://alpha.histaff.vn',
      pushSubscriptionUser: 'tannv',
      email: 'me@example.com',
      stepName: 'Initiator Approval',
      stepOrder: 0,
      stepType: 'single',
      reacted: false,
      portrait: 'https://miukafoto.com/Content/shared_pictures/30a2e3e4-3965-42e6-8d0d-56014ab45021_20250328041533_me.png'
    },
    {
      id: 'b0d9b6f1-8940-40b0-b1f2-df804b17a809',
      name: 'My Father',
      email: 'father@example.com',
      stepName: 'Parent Review',
      stepOrder: 1,
      stepType: 'short_circuit',
      reacted: false,
      portrait: 'https://miukafoto.com/Content/shared_pictures/30a2e3e4-3965-42e6-8d0d-56014ab45021_20250328042034_father.png'
    },
    {
      id: 'df6deefc-7797-41e1-99a7-299b5b47eb38',
      name: 'My Mother',
      email: 'mother@example.com',
      stepName: 'Parent Review',
      stepOrder: 1,
      stepType: 'short_circuit',
      reacted: false,
      portrait: 'https://miukafoto.com/Content/shared_pictures/30a2e3e4-3965-42e6-8d0d-56014ab45021_20250328042218_mother.png'
    },
    {
      id: '98f9fd8b-63cc-40d9-952e-51ff493da335',
      name: 'My Wife',
      email: 'wife@example.com',
      stepName: 'Spouse Approval',
      stepOrder: 2,
      stepType: 'single',
      reacted: false,
      portrait: 'https://miukafoto.com/Content/shared_pictures/30a2e3e4-3965-42e6-8d0d-56014ab45021_20250328042634_wife.png'
    },
    {
      id: 'ced93436-b11a-4a23-9f8b-0fb0ece99b85',
      name: 'My Son',
      email: 'son@example.com',
      stepName: 'Children Approval',
      stepOrder: 3,
      stepType: 'parallel',
      reacted: false,
      portrait: 'https://miukafoto.com/Content/shared_pictures/30a2e3e4-3965-42e6-8d0d-56014ab45021_20250328043033_son.png'
    },
    {
      id: '904efa6e-6654-4b92-8617-01a1b6c7b18a',
      name: 'My Daughter',
      email: 'daughter@example.com',
      stepName: 'Children Approval',
      stepOrder: 3,
      stepType: 'parallel',
      reacted: false,
      portrait: 'https://miukafoto.com/Content/shared_pictures/30a2e3e4-3965-42e6-8d0d-56014ab45021_20250328043316_daughter.png'
    }
  ];

  private http = inject(HttpClient);
  private workflowAdminService = inject(WorkflowAdminService)
  api: string = 'http://159.223.59.17:5000/api/Workflow';
  sagaStatus: string = '';

  intervalId: any;

  showLeftBar(): void {
    this.leftBarShown = true;
  }

  ngAfterViewInit(): void {

    setTimeout(() => {
      this.getActiveCorrelationId();
    })

    // ⏱ Optional: Poll every 10s
    /*
    this.intervalId = setInterval(() => {
      if (this.saga?.correlationId)
        this.loadSagaState(this.saga.correlationId);
    }, 10000);
    */
  }

  getActiveCorrelationId() {
    this.subscriptions.push(
      this.http.get<string>(`${this.api}/active-correlation-id`)
        .subscribe({
          next: (correlationId: string) => {
            this.loadSagaState(correlationId); // dynamically load saga
          },
          error: (err: any) => {
            this.saga = null;
            this.sagaStatus = 'Not loaded';
            console.error('❌ Could not fetch active correlation ID:', err);
            this.sagaStatus = 'No active saga loaded';
          }
        })
    );
  }

  loadSagaState(correlationId: string): void {
    this.subscriptions.push(
      this.http.get<ISaga>(`${this.api}/state/${correlationId}`).subscribe({
        next: (state: ISaga) => {
          this.saga = state;
          this.sagaStatus = `Current Step: ${state.currentStep} | Fully Approved: ${state.isFullyApproved}`;
          if (this.saga.currentState === 'Rejected') {
            alert(`Opppp! Your request has been rejected by ${this.saga.lastApprovedByEmail}!!`);
            this.onReset(true);
          } else if (this.saga.currentState === 'Completed') {
            alert("Congratulation! Your request has been fully approved!!")
          }
        },
        error: (err: any) => {
          console.error('Failed to load saga state:', err);
          this.sagaStatus = 'Failed to load saga state';
        }
      })
    );
  }

  approve(actor: any) {
    this.sendDecision(actor, true);
  }

  reject(actor: any) {
    this.sendDecision(actor, false);
  }

  sendDecision(actor: any, isApproved: boolean) {

    switch (actor.name) {
      case 'Me':
        if (!this.saga) {
          const payload = {
            RequestedByEmail: actor.email,
            requestedAt: new Date().toISOString(),
            reason: isApproved ? 'Kick off approved' : 'Kick off rejected',
            isApproved
          };

          this.subscriptions.push(
            this.http.post(`${this.api}/kickoff`, payload).subscribe({
              next: (newCorrelationId: any) => {
                this.loadSagaState(newCorrelationId); // Load saga from backend
                alert(`${actor.name} has kicked off the workflow`);
                this.stepType = actor.stepType;
                if (this.stepType != EnumStepType.PARALLEL) {
                  this.lastCompletedStepOrder += 1;
                }
              },
              error: (err: any) => console.error('Kickoff failed', err)
            })
          );
        } else {
          alert("You have already sent the request!")
        }
        break;
      default:
        if (!this.saga) {
          alert('Saga not loaded yet.');

        } else {
          const payload = {
            correlationId: this.saga.correlationId,
            stepName: actor.stepName,
            performedByEmail: actor.email,
            performedAt: new Date().toISOString(),
            isApproved,
            reason: isApproved ? 'Approved by UI' : 'Rejected by UI'
          };

          this.subscriptions.push(
            this.http.post(`${this.api}/step-completed`, payload)
              .subscribe({
                next: () => {
                  setTimeout(() => this.loadSagaState(this.saga!.correlationId));
                  alert(`${actor.name} decision sent`);
                  this.actors.filter(x => x.id === actor.id)[0].reacted = true;
                  this.stepType = actor.stepType;
                  if (this.stepType != EnumStepType.PARALLEL) {
                    this.lastCompletedStepOrder += 1;
                  }
                },
                error: (err: any) => console.error('❌ Backend error', err)
              })
          );

        }

        break;
    }

  }

  onReset(noConfirm: boolean = false): void {

    const confirmed = noConfirm || confirm('Are you sure you want to reset all workflow data?');

    if (confirmed) {
      this.workflowAdminService.resetWorkflow().subscribe({
        next: (res: any) => {
          console.log('✅ Reset successful', res);
          this.getActiveCorrelationId();
          this.actors.forEach(x => x.reacted = false);
          this.lastCompletedStepOrder = -1;
          this.stepType = EnumStepType.SINGLE;
        },
        error: (err: any) => {
          console.error('❌ Reset failed', err);
        }
      });
    }
  }

  onCloseDiagram(): void {
    this.leftBarShown = false;
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(x => x?.unsubscribe());
    if (!!this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

}