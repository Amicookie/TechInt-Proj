import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { IFile } from './file';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  serverData: JSON;

  public _url: string = "http://127.0.0.1:5000/files";

  constructor(private http: HttpClient) { }

  getFiles(): Observable<IFile[]>{
    return this.http.get<IFile[]>(this._url).pipe(catchError(this.errorHandler));
  }

  getFile(file_id): Observable<IFile[]>{
    return this.http.get<IFile[]>(this._url+"/"+file_id).pipe(catchError(this.errorHandler));
  }

  editFile(file_id, file_name, file_content, user_id){
    let encoded_data = JSON.stringify({file_id, file_name, file_content, user_id});
    console.log(encoded_data)
    let headers = new HttpHeaders().set("Content-Type", "application/json;charset=utf-8");
    return this.http.put(this._url+"/"+file_id, encoded_data, {headers})
                    .subscribe(data => {console.log("PUT request is successful! ", encoded_data);},
                    error => {console.log("Error", error);
                    complete => {console.log("complete", complete)}});
  }

  createFile(file_name, file_content, user_id){//: Observable<Object> {
    let encoded_data = JSON.stringify({file_name, file_content, user_id});
    console.log(encoded_data)
    let headers = new HttpHeaders().set("Content-Type", "application/json;charset=utf-8");
    // return this.http.post(encoded_data, this._url+"/files/create", {headers}).map(
    //   (res: Response) => res.json() || {}
    // );
    return this.http.post(this._url, encoded_data, {headers})
                    .subscribe(data => {console.log("POST request is successful! ", encoded_data);},
                    error=> {console.log("Error", error);});
  }

  errorHandler(error: HttpErrorResponse){
    return throwError(error.message || "Server error"); 
  }
}
