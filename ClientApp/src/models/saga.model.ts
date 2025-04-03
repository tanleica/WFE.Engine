export interface ISaga {
    correlationId: string;
    currentState: string;
    currentStep?: string;

    requestedByEmail?: string;
    requestedAt?: Date;

    isFullyApproved: boolean;
    lastApprovedByEmail?: string;
    lastActionAt?: string;
}
