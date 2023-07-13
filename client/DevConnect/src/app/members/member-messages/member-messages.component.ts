import { Component, Input, OnInit, ViewChild, ElementRef, ChangeDetectionStrategy } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() username?: string; 
  @ViewChild('messageForm') messageForm? : NgForm;
  
  messageBody='';

  constructor(public messageService:MessageService){}

  ngOnInit(): void {
    
  }

  sendMessage(){
    if(!this.username) return;
    // this.messageService.sendMessage(this.username,this.messageBody).subscribe({
    //   next: message =>{
    //     this.messages.push(message);
    //     this.messageForm?.reset();
    //   }
    // });
    this.messageService.sendMessage(this.username,this.messageBody).then(() =>{
      this.messageForm?.reset();
    })
  }

 
 

}
