import { Component, Input, OnInit, ViewChild, ElementRef } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() username?: string;
  @Input() messages : Message[]=[];
  @ViewChild('messageForm') messageForm? : NgForm;
  
  messageBody='';

  constructor(private messageService:MessageService){}

  ngOnInit(): void {
    
  }

  senMessage(){
    if(!this.username) return;
    this.messageService.sendMessage(this.username,this.messageBody).subscribe({
      next: message =>{
        this.messages.push(message);
        this.messageForm?.reset();
      }
    });
  }

 
 

}
