import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './layout/layout.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { ChatComponent } from './chat/chat.component';

@NgModule({
  declarations: [LayoutComponent, HeaderComponent, FooterComponent, ChatComponent],
  imports: [
    CommonModule
  ],
  exports: [
    LayoutComponent
  ]
})
export class UiModule { }
