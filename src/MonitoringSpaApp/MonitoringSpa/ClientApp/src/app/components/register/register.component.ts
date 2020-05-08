import { Component, OnInit, OnDestroy, Input } from '@angular/core';

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from '../../services/auth.service';
import { ConfigurationService } from '../../services/configuration.service';
import { Utilities } from '../../services/utilities';
import { UserRegister } from '../../models/user-register.model';

@Component({
  selector: 'app-register',
    templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})

export class RegisterComponent implements OnInit, OnDestroy {

    userRegister = new UserRegister();
    isLoading = false;
    formResetToggle = true;
    modalClosedCallback: () => void;
    loginStatusSubscription: any;

    @Input()
    isModal = false;


    constructor(private alertService: AlertService, private authService: AuthService, private configurations: ConfigurationService) {

    }


    ngOnInit() {
        if (this.getShouldRedirect()) {
            this.authService.redirectLoginUser();
        } else {
            this.loginStatusSubscription = this.authService.getLoginStatusEvent().subscribe(isLoggedIn => {
                if (this.getShouldRedirect()) {
                    this.authService.redirectLoginUser();
                }
            });
        }
    }


    ngOnDestroy() {
        if (this.loginStatusSubscription) {
            this.loginStatusSubscription.unsubscribe();
        }
    }


    getShouldRedirect() {
        return !this.isModal && this.authService.isLoggedIn && !this.authService.isSessionExpired;
    }


    showErrorAlert(caption: string, message: string) {
        this.alertService.showMessage(caption, message, MessageSeverity.error);
    }

    closeModal() {
        if (this.modalClosedCallback) {
            this.modalClosedCallback();
        }
    }


    register() {
        this.isLoading = true;
        this.alertService.startLoadingMessage('', 'Attempting login...');
        this.authService.registerUser(this.userRegister)
            .subscribe(
                resp => {
                    setTimeout(() => {
                        this.alertService.stopLoadingMessage();
                        this.isLoading = false;
                        this.reset();
                        this.alertService.showMessage('Register', `Successfully registered.`, MessageSeverity.success);
                        this.authService.gotoPage("/login");
                    }, 500);
                },
                error => {

                    this.alertService.stopLoadingMessage();

                    if (Utilities.checkNoNetwork(error)) {
                        this.alertService.showStickyMessage(Utilities.noNetworkMessageCaption, Utilities.noNetworkMessageDetail, MessageSeverity.error, error);
                    } else {
                        const errorMessage = Utilities.getHttpResponseMessage(error);

                        if (errorMessage) {
                            this.alertService.showStickyMessage('Unable to register', this.mapLoginErrorMessage(errorMessage), MessageSeverity.error, error);
                        } else {
                            this.alertService.showStickyMessage('Unable to register', 'An error occured while registering, please try again later.\nError: ' + Utilities.getResponseBody(error), MessageSeverity.error, error);
                        }
                    }

                    setTimeout(() => {
                        this.isLoading = false;
                    }, 500);
                });
    }

    mapLoginErrorMessage(error: string) {

        if (error == 'invalid_username_or_password') {
            return 'Invalid username or password';
        }

        if (error == 'invalid_grant') {
            return 'This account has been disabled';
        }

        return error;
    }


    reset() {
        this.formResetToggle = false;

        setTimeout(() => {
            this.formResetToggle = true;
        });
    }
}
