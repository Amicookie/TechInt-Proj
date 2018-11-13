import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
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

  errorHandler(error: HttpErrorResponse){
    return throwError(error.message || "Server error"); 
  }
}
