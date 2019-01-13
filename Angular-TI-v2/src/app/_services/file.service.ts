import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { IFile } from '../_models/file';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { WebsocketService } from '../_services/websocket.service';
import { showToast } from '../toaster-helper';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  serverData: JSON;

  public _url: string = "http://127.0.0.1:5000/files";
  public data$: BehaviorSubject<any> = new BehaviorSubject({});

  constructor(private http: HttpClient, 
    private websocketservice: WebsocketService) { }

  getFiles(): Observable<IFile[]>{
    return this.http.get<IFile[]>(this._url).pipe(catchError(this.errorHandler));
  }

  getFile(file_id): Observable<IFile[]>{
    return this.http.get<IFile[]>(this._url+"/"+file_id).pipe(catchError(this.errorHandler));
  }

  // updateData(){
  //   let data = this.getFiles().do((data) => {
  //     this.data$.next(data);
  //   })
  // }

  editFile(file_id, file_name, file_content, user_id){
    const self = this;
    let encoded_data = JSON.stringify({file_id, file_name, file_content, user_id});
    console.log(encoded_data)
    let headers = new HttpHeaders().set("Content-Type", "application/json;charset=utf-8");
    return this.http.put(this._url+"/"+file_id, encoded_data, {headers})
                    .subscribe(
                      data => {console.log("PUT request is successful! ", encoded_data);
                      },
                      // updatedFile => {
                      //   if (updatedFile) {
                      //     self.websocketservice.emitEventOnFileUpdated(updatedFile); // emit event
                      //   } else {
                      //     // on error
                      //     self.toastr.error('Update error!', 'An error occurred when updating your file, please try again!');
                      //   }
                      // },
                    error => {console.log("Error", error);
                    complete => {
                      console.log("complete", complete);
                      if (complete) {
                        self.websocketservice.emitEventOnFileUpdated(file_name, user_id);
                      } else {
                        //on error
                        showToast("An error occurred while updating your file, please try again!")
                        
                        //self.toastr.error({title: 'Update error!', msg:''});
                      }
                    }});
  }

  createFile(file_name, file_content, user_id){//: Observable<Object> {
    const self = this;
    let encoded_data = JSON.stringify({file_name, file_content, user_id});
    console.log(encoded_data)
    let headers = new HttpHeaders().set("Content-Type", "application/json;charset=utf-8");
    // return this.http.post(encoded_data, this._url+"/files/create", {headers}).map(
    //   (res: Response) => res.json() || {}
    // );
    return this.http.post(this._url, encoded_data, {headers})
                    .subscribe(data => {console.log("POST request is successful! ", encoded_data);},
                    error=> {console.log("Error", error);
                    complete => {
                      console.log("complete", complete);
                      if (complete) {
                        self.websocketservice.emitEventOnFileSaved(file_name);
                      } else {
                        //on error

                        showToast("An error occurred while saving your file, please try again!")
                      }
                    }});
  }

  errorHandler(error: HttpErrorResponse){
    return throwError(error.message || "Server error"); 
  }
}
