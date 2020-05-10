export class TelemetryRecord {

    constructor(id: string, controllerRegistryId?: string, controllerRegistry?: any, imageUrl?: string, counterValue?: number, processedSuccessful?: boolean, createdDate?: string) {
        this.id = id;
        this.controllerRegistryId = controllerRegistryId;
        this.controllerRegistry = controllerRegistry;
        this.imageUrl = imageUrl;
        this.counterValue = counterValue;
        this.processedSuccessful = processedSuccessful;
        this.createdDate = createdDate;
    }

    id: string;
    controllerRegistryId: string;
    controllerRegistry: any;
    imageUrl: string;
    counterValue: number;
    processedSuccessful: boolean;
    createdDate: string;
}
