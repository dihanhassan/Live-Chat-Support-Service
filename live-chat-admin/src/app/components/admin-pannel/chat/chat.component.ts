import { Component, AfterViewChecked, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChatService } from '../../../services/chat.service';
import { Message } from '../../../models/message';  // Adjust the import path according to your project structure

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']  // Use 'styleUrls' instead of 'styleUrl'
})
export class ChatComponent implements OnInit, AfterViewChecked {
  chatService = inject(ChatService);
  inputMessage = "";
  messages: Message[] = [];
  router = inject(Router);
  actRoute = inject(ActivatedRoute);
  loggedInUserName = sessionStorage.getItem("user");
  roomName = sessionStorage.getItem("room");
  userName: string = "";
  roomID: string = "";
  userEmail: string = "";

  @ViewChild('scrollMe') private scrollContainer!: ElementRef;
  connected: boolean = false;

  ngOnInit(): void {

    console.log(this.messages)
    console.log(this.loggedInUserName);
    
    console.log(this.loggedInUserName);
    this.actRoute.queryParams.subscribe(params => {
      const user = {
        item1: params['name'],
        item2: params['id'],
        item3: params['email']
      };
      this.roomID = user.item2;
      this.userEmail = user.item3;
    
      // this.chatService.removeAdmin(this.loggedInUserName??" ",this.roomID,1);

      console.log(this.userEmail);
      console.log(this.roomID);
      
      this.chatService.messages$.subscribe((messages: Message[]) => {
        console.log(this.messages);
        console.log(this.userEmail);
        this.messages = messages.filter(message => message.room === this.roomID);
        console.log(this.messages);
      });
    });

  

    this.chatService.connectedUsers$.subscribe(res => {
      console.log(res);
    });
  }

  

  ngAfterViewChecked(): void {
    this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
  }

  sendMessage() {
    this.chatService.sendMessage(this.inputMessage, this.roomID).then(() => {
      this.inputMessage = "";
    }).catch(() => {
      console.log('error');
    });
  }

  leaveChat() {
    this.connected = false;
    this.chatService.leaveChat().then(() => {
      this.router.navigate(['admin-pannel/dashboard']);
    }).catch(() => {
      console.log('error');
    });
  }
}
