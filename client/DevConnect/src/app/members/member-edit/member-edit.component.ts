import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { DefaultUrlSerializer } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  @ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadnotification($event:any){
    if(this.editForm?.dirty)
    {
      $event.returnValue = true;
    }
  }
  member: Member | undefined;
  user :User | null = null;

  constructor(private accountService: AccountService, 
              private memberService:MembersService, 
              private toastr:ToastrService){
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    });
  }
  ngOnInit(): void {
    this.loadMember(false);
  }

  loadMember(isChanged : boolean){
    if(!this.user) return;
    this.memberService.getMember(this.user.username, isChanged).subscribe({
      
      next: member => {
        console.log(": ");
        console.log(member);
        this.member = member       
      }
    })
  }

  updateMember()
  {   
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        if(this.user && this.member){
          console.log(this.member);
          this.member.fullName = this.editForm?.value['fullname'];
          this.accountService.setCurrentUser(this.user);
        }
        this.loadMember(true);
        this.toastr.success('Updated Successfully');
        this.editForm?.reset(this.member);
      }
    })
   
  }

}
