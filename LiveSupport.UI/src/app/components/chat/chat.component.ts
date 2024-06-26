import { Component } from '@angular/core';
import { AfterViewChecked, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { ChatService } from '../../services/chat.service';
import { Router ,NavigationStart } from '@angular/router';
import { filter } from 'rxjs/operators';
@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent implements OnInit ,AfterViewChecked{
  chatService = inject(ChatService);
  inputMessage = "";
  messages: any[] = [];
  router = inject(Router);
  loggedInUserName = sessionStorage.getItem("user");
  roomName = sessionStorage.getItem("room");
  
  
  /**
   *
   */
 

  @ViewChild('scrollMe') private scrollContainer!: ElementRef;
  
  ngOnInit(): void {
    
    console.log(this.loggedInUserName);

    this.chatService.messages$.subscribe((messages)=>{
      this.messages = messages;
    });
    this.chatService.connectedUsers$.subscribe(res=>{
      
      console.log(res);

    });
  }

  ngAfterViewChecked(): void {
    this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
  }
  sendMessage(){
    this.chatService.sendMessage(this.inputMessage).then(()=>{
      this.inputMessage = "";
    }).catch(()=>{
      console.log('error');
    });
  }
  leaveChat(){
    console.log("Here");
    this.chatService.leaveChat().then(()=>{
      this.router.navigate(['welcome']);
    }).catch(()=>{
      console.log('error');
    });
  }
  

}
