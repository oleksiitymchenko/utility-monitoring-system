import { Component, OnInit, OnDestroy, Input, ViewChild, TemplateRef } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AuthService } from '../../services/auth.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { Utilities } from '../../services/utilities';
import { ControllerRegistryService } from '../../services/controller-registry.service';
import { ControllerRegistry } from '../../models/controller-registry';

@Component({
  selector: 'app-registry',
  templateUrl: './registry.component.html',
  styleUrls: ['./registry.component.scss'],
  animations: [fadeInOut]
})
export class RegistryComponent implements OnInit, OnDestroy {

    rows = [];
    rowsCache = [];
    columns = [];
    editing = {};
    registryEdit: any = {};
    isDataLoaded = false;
    loadingIndicator = true;
    formResetToggle = true;
    _currentUserId: string;

    get currentUserId() {
        if (this.authService.currentUser) {
            this._currentUserId = this.authService.currentUser.id;
        }

        return this._currentUserId;
    }

    @Input()
    verticalScrollbar = false;

    @ViewChild('idHeaderTemplate', { static: true })
    idHeaderTemplate: TemplateRef<any>;

    @ViewChild('idTemplate', { static: true })
    idTemplate: TemplateRef<any>;

    @ViewChild('nameTemplate', { static: true })
    nameTemplate: TemplateRef<any>;

    @ViewChild('descriptionTemplate', { static: true })
    descriptionTemplate: TemplateRef<any>;

    @ViewChild('actionsTemplate', { static: true })
    actionsTemplate: TemplateRef<any>;

    @ViewChild('editorModal', { static: true })
    editorModal: ModalDirective;

    constructor(
        private alertService: AlertService,
        private authService: AuthService,
        private registryService: ControllerRegistryService) {
    }

    ngOnInit() {
        this.loadingIndicator = true;

        this.fetch();

        this.columns = [
            { prop: 'id', name: '', width: 300, headerTemplate: this.idHeaderTemplate, cellTemplate: this.idTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false },
            { prop: 'name', name: 'Name', cellTemplate: this.nameTemplate, width: 200 },
            { prop: 'description', name: 'Description', cellTemplate: this.descriptionTemplate, width: 500 },
            { name: '', width: 50, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
        ];
    }

    ngOnDestroy() {
    }

    fetch() {
        this.registryService.getAll().subscribe(
            (data) => {
                console.log(data);
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

    onSearchChanged(value: string) {
        this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description) || value == 'important' && r.important || value == 'not important' && !r.important);
    }

    showErrorAlert(caption: string, message: string) {
        this.alertService.showMessage(caption, message, MessageSeverity.error);
    }

    addRegistry() {
        this.formResetToggle = false;

        setTimeout(() => {
            this.formResetToggle = true;

            this.registryEdit = {};
            this.editorModal.show();
        });
    }

    save() {
        console.log(this.registryEdit);
        this.registryService
            .create(Object.assign({}, this.registryEdit))
            .subscribe(
                result => {
                    this.rowsCache.splice(0, 0, this.registryEdit);
                    this.rows.splice(0, 0, this.registryEdit);
                    this.refreshDataIndexes(this.rowsCache);
                    this.rows = [...this.rows];
                    this.registryEdit = {};
                },
                error => { this.showErrorAlert("Unable to create", error) });
        this.editorModal.hide();
    }

    updateValue(event, cell, cellValue, row) {
        console.log(row);
        const x = new ControllerRegistry(row.id, row.name, row.description);
        this.registryService
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
        this.alertService.showDialog('Are you sure you want to delete the task?', DialogType.confirm, () => this.deleteHelper(row));
    }

    deleteHelper(row) {
        this.registryService
            .delete(row.id)
            .subscribe(
                res => {
                    this.rowsCache = this.rowsCache.filter(item => item !== row);
                    this.rows = this.rows.filter(item => item !== row);
                },
                error => { this.showErrorAlert("Unable to delete", error) });
    }
}
