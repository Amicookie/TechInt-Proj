import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataSharingService {
  public isUserLoggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  
  constructor() { 
    if(localStorage.getItem('user_id')==""){
      this.isUserLoggedIn.next(false);
    } else {
      this.isUserLoggedIn.next(true);
    }
   }

  
}
