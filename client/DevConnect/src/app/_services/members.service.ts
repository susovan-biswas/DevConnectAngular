import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { BehaviorSubject, Observable, map, of, take } from 'rxjs';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Message } from '../_models/message';



@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  
 

  members: Member[] = [];
  memberCache = new Map();
  user:User | undefined;
  userParams: UserParams |undefined;


  constructor(private http: HttpClient, private accountService:AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user =>{
        if(user){
          this.userParams = new UserParams(user)
        }
      }
    });
   }



  

   getUserParams(){
    return this.userParams;
   }

   setUserParams(params : UserParams){
    this.userParams = params;
   }

   resetUserParams(){
    if(this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
   }

  getMembers(userParams: UserParams) {
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);
    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    return getPaginatedResults<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }



  getMember(userName: String, isChanged: boolean = false) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === userName);
    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + userName);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }

  setProfilePic(photoId: number) {
    return this.http.put(this.baseUrl + 'users/setProfilePic/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/deletePhoto/' + photoId, {});
  }

  addLike(username: string){
    return this.http.post(this.baseUrl+'likes/'+username,{});
  }

  getLikes(predicate: string, pageNumber: number, pageSize:number){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginatedResults<Member[]>(this.baseUrl+'likes',params, this.http)
  }

  





}
