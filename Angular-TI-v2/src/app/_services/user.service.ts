import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { IUser } from '../_models/user';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { showToast } from '../toaster-helper';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  serverData: JSON;
  

  public currentUserSubject: BehaviorSubject<IUser>;
  public currentUser: Observable<IUser>;
  public usersCurrentlyLoggedIn = [];


  public _url: string = "http://127.0.0.1:5000/users";

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<IUser>(JSON.parse(localStorage.getItem('user_id')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): IUser {
    return this.currentUserSubject.value;
  }

  login(user_login, user_password) {
    let encoded_data = JSON.stringify({user_login, user_password});
    let headers = new HttpHeaders().set("Content-Type", "application/json;charset=utf-8");
    return this.http.post<any>(this._url, encoded_data, {headers})
    .pipe(map(user => {
        if (user && user.user_exists == 1 && user.logged_in == 1){
          localStorage.setItem('currentUser', user_login);
          localStorage.setItem('user_id', user.user_id);
          this.usersCurrentlyLoggedIn.push(user_login);
          //this.currentUserSubject.next(user);
          showToast("Succesfully logged in!");
        } else if (user.user_exists == 0) {
          showToast("User doesn't exist. Please try again.");
        } else if (user.logged_in == 0) {
          showToast("Password is not correct. Please try again.");
        } else {
          showToast("Problem z warunkami xd");
        }
    }))
  }

  logout() {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('user_id');
    //this.currentUserSubject.next(null);
    showToast("Succesfully logged out!");
  }

  getUsers(): Observable<IUser[]>{
    return this.http.get<IUser[]>(this._url)
                    .pipe(catchError(this.errorHandler));
  }

  errorHandler(error: HttpErrorResponse){
    return throwError(error.message || "Server error"); 
  }
}
