import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { User } from '../_models/user';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?:HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http:HttpClient) { }

  createHubConnection(user:User, otherUserName:string){

    this.hubConnection = new HubConnectionBuilder()
       .withUrl(this.hubUrl+'message?user='+otherUserName,{
        accessTokenFactory:() =>user.token
       })
       .withAutomaticReconnect()
       .build();

       this.hubConnection.start().catch(error => console.log(error));

       this.hubConnection.on('RecieveMessageThread', messages =>{
        this.messageThreadSource.next(messages);        
       });

       this.hubConnection.on('UpdatedGroup', (group:Group) =>{
        if(group.connections.some(X=>X.username === otherUserName)){
          this.messageThread$.pipe(take(1)).subscribe({
            next: messages =>{
              messages.forEach( message => {
                if(!message.dateRead){
                  message.dateRead = new Date(Date.now())
                }
              })
              this.messageThreadSource.next([...messages]);
            }
          });
        }        
       });

       this.hubConnection.on('NewMessage', message =>{
        
        this.messageThread$.pipe(take(1)).subscribe({
          next: messages =>{
             this.messageThreadSource.next([...messages, message]);           
          }
        });
       })

   }

   stopHubConnection(){
    if(this.hubConnection){
      this.hubConnection.stop().catch(error => console.log(error));
    }
    
   }

  getMessages(pageNumber:number, pageSize:number, container:string){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResults<Message[]>(this.baseUrl+'messages', params, this.http);
  }

  getMessageThread(username:string){
    return this.http.get<Message[]>(this.baseUrl+'messages/thread/'+username);

  }

  async sendMessage(username: string, messageBody:string){
     //const params={'recipient': username, 'messagebody':messageBody};     
    // return this.http.post<Message>(this.baseUrl+'messages', params);
    return this.hubConnection?.invoke('SendMessage', {recipient: username, messageBody})
    .catch(error=>console.log(error))
    
  }

  deleteMessage(id : number){
   return this.http.delete(this.baseUrl+'messages/'+id);

  }
}
