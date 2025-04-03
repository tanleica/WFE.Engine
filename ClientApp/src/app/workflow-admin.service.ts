import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class WorkflowAdminService {

    api: string = 'http://159.223.59.17:5000/api/Workflow';

    constructor(private http: HttpClient) { }

    resetWorkflow(): Observable<any> {
        return this.http.post(`${this.api}/reset`, {});
    }
}
