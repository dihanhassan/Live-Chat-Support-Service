import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ChatService } from '../../services/chat.service';
@Component({
  selector: 'app-join-room',
  templateUrl: './join-room.component.html',
  styleUrl: './join-room.component.scss'
})
export class JoinRoomComponent implements OnInit{
  joinRoomForm!: FormGroup;
  fb = inject(FormBuilder);
  router = inject(Router);
  chatService = inject(ChatService);
  ngOnInit(): void {
    this.joinRoomForm = this.fb.group({
      user: ['', Validators.required],
      // room: ['', Validators.required],
      email: ['', Validators.required],
    });
  }
  joinRoom(){
    const {user,email} = this.joinRoomForm.value;
    sessionStorage.setItem('user',user);
    // sessionStorage.setItem("room", room);
    sessionStorage.setItem("email", email);
    console.log(user);
    this.chatService.joinRoom(user,email,1).then(()=>{
      this.router.navigate(['chat']);
    }).catch((error)=>{
      console.log(error);
    });

  }
}
