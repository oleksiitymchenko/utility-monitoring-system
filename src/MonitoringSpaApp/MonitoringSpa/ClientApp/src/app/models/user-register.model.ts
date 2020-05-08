export class UserRegister {

    constructor(userName?: string, password?: string, email?: string, roles?: []) {
        this.userName = userName;
        this.password = password;
        this.email = email;
        this.roles = roles;
    }

    userName: string;
    password: string;
    email: string;
    roles: [];
}
