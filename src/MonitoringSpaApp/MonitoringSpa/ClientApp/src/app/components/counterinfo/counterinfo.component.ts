import { Component, OnInit, OnDestroy, Input, ViewChild, TemplateRef } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AuthService } from '../../services/auth.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { Utilities } from '../../services/utilities';
import { ControllerRegistry } from '../../models/controller-registry';
import { TelemetryRecordService } from '../../services/telemetry-record.service';
import { TelemetryRecord } from '../../models/telemetry-record';

@Component({
  selector: 'app-counterinfo',
  templateUrl: './counterinfo.component.html',
  styleUrls: ['./counterinfo.component.scss'],
  animations: [fadeInOut]
})

export class CounterinfoComponent implements OnInit, OnDestroy {

    rows = [];
    rowsCache = [];
    columns = [];
    editing = {};
    telemetryDataEdit: any = {};
    isDataLoaded = false;
    loadingIndicator = true;
    formResetToggle = true;
    _currentUserId: string;
    currentImageUrl: string;

    get currentUserId() {
        if (this.authService.currentUser) {
            this._currentUserId = this.authService.currentUser.id;
        }

        return this._currentUserId;
    }

    @Input()
    verticalScrollbar = false;

    @ViewChild('controllerNameHeaderTemplate', { static: true })
    controllerNameHeaderTemplate: TemplateRef<any>;

    @ViewChild('controllerNameTemplate', { static: true })
    controllerNameTemplate: TemplateRef<any>;

    @ViewChild('counterValueTemplate', { static: true })
    counterValueTemplate: TemplateRef<any>;

    @ViewChild('processedSuccessfulTemplate', { static: true })
    processedSuccessfulTemplate: TemplateRef<any>;

    @ViewChild('actionsTemplate', { static: true })
    actionsTemplate: TemplateRef<any>;

    @ViewChild('dateTemplate', { static: true })
    dateTemplate: TemplateRef<any>;
    
    @ViewChild('editorModal', { static: true })
    editorModal: ModalDirective;

    constructor(
        private alertService: AlertService,
        private authService: AuthService,
        private telemetryService: TelemetryRecordService) {
    }

    ngOnInit() {
        this.loadingIndicator = true;

        this.fetch();

        this.columns = [
            { prop: 'controllerRegistry.name', name: '', width: 300, headerTemplate: this.controllerNameHeaderTemplate, cellTemplate: this.controllerNameTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false },
            { prop: 'counterValue', name: 'Counter value', cellTemplate: this.counterValueTemplate, width: 200 },
            { prop: 'processedSuccessful', name: 'Processed successful', cellTemplate: this.processedSuccessfulTemplate, width: 150 },
            { prop: 'createdDate', name: 'Date of capturing', cellTemplate: this.dateTemplate, width: 350 },
            { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
        ];
    }

    ngOnDestroy() {
    }

    fetch() {
        this.telemetryService.getAll().subscribe(
            (data) => {
                console.log(data);
                data = data.map(d => {
                    d.createdDate = new Date(d.createdDate).toLocaleString();
                    return d;
                });
                this.refreshDataIndexes(data);
                this.rows = data;
                this.rowsCache = [...data];
                this.isDataLoaded = true;
                setTimeout(() => { this.loadingIndicator = false; }, 1500);
            },
            error => {
                this.isDataLoaded = true;
                setTimeout(() => { this.loadingIndicator = false; }, 1500);
            });
    }

    refreshDataIndexes(data) {
        let index = 0;

        for (const i of data) {
            i.$$index = index++;
        }
    }

    onSearchChanged(searchValue: string) {
        if (!searchValue) {
            this.rows = this.rowsCache;
        }
        searchValue = searchValue.toLowerCase();
        this.rows = this.rowsCache.filter(r => {
            const keys = Object.keys(r);
            let including = false;
            keys.forEach(key => {
                if (!r[key]) return;
                if (key.toString().toLowerCase().includes("image") || key.toString().toLowerCase().includes("index")) return;
                const include = r[key].toString().toLowerCase().startsWith(searchValue);
                if (include) including = true;
            });
            return including;
        });
    }

    showErrorAlert(caption: string, message: string) {
        this.alertService.showMessage(caption, message, MessageSeverity.error);
    }

    showPhoto(row) {
        this.currentImageUrl = row.imageUrl;
        setTimeout(() => {
            this.formResetToggle = true;
            this.telemetryDataEdit = {};
            this.editorModal.show();
        });
    }

    save() {
        console.log(this.telemetryDataEdit);
        this.telemetryService
            .create(Object.assign({}, this.telemetryDataEdit))
            .subscribe(
                result => {
                    this.rowsCache.splice(0, 0, this.telemetryDataEdit);
                    this.rows.splice(0, 0, this.telemetryDataEdit);
                    this.refreshDataIndexes(this.rowsCache);
                    this.rows = [...this.rows];
                    this.telemetryDataEdit = {};
                },
                error => { this.showErrorAlert("Unable to create", error) });
        this.editorModal.hide();
    }

    updateValue(event, cell, cellValue, row) {
        console.log(row);
        const x = new TelemetryRecord(row.id, row.controllerRegistryId, null, row.imageUrl, row.counterValue, row.processedSuccessful);
        this.telemetryService
            .put(row.id, x)
            .subscribe(
                res => {
                    this.editing[row.$$index + '-' + cell] = false;
                    this.rows[row.$$index][cell] = event.target.value;
                    this.rows = [...this.rows];
                },
                error => { console.log(error); this.showErrorAlert("Unable to update", error); });
    }

    delete(row) {
        this.alertService.showDialog('Are you sure you want to delete the counterinfo?', DialogType.confirm, () => this.deleteHelper(row));
    }

    deleteHelper(row) {
        this.telemetryService
            .delete(row.id)
            .subscribe(
                res => {
                    this.rowsCache = this.rowsCache.filter(item => item !== row);
                    this.rows = this.rows.filter(item => item !== row);
                },
                error => { this.showErrorAlert("Unable to delete", error) });
    }
}
