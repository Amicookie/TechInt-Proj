import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { FileComponent } from './_components/file/file.component';
import { UserComponent } from './_components/user/user.component';
import { UiModule } from './_components/ui/ui.module';

import { HttpClientModule, HttpClientJsonpModule } from '@angular/common/http';
import { LoginComponent } from './_components/login/login.component';

import { WebsocketService } from './_services/websocket.service';
import { ChatService } from './_services/chat.service';


@NgModule({
  declarations: [
    AppComponent,
    FileComponent,
    UserComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    HttpClientJsonpModule,
    AppRoutingModule,
    UiModule
  ],
  providers: [
    ChatService,
    WebsocketService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
