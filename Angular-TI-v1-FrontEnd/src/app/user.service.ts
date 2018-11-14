import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { IUser } from './user';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  serverData: JSON;
  

  public _url: string = "http://127.0.0.1:5000/users";

  constructor(private http: HttpClient) { }

  getUsers(): Observable<IUser[]>{
    return this.http.get<IUser[]>(this._url)
                    .pipe(catchError(this.errorHandler));
  }

  handleLogin(user_login, user_password){
    let encoded_data = JSON.stringify({user_login, user_password});
    console.log(encoded_data)
    let headers = new HttpHeaders().set("Content-Type", "application/json;charset=utf-8");
    return this.http.post(this._url, encoded_data, {headers})
                    .subscribe(data => {console.log("POST request is successful! ", encoded_data);},
                    error=> {console.log("Error", error);});
    
  }

  

  errorHandler(error: HttpErrorResponse){
    return throwError(error.message || "Server error"); 
  }
}
