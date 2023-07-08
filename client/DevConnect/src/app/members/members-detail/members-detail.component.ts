import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-members-detail',
  templateUrl: './members-detail.component.html',
  styleUrls: ['./members-detail.component.css']
})
export class MembersDetailComponent implements OnInit {

@ViewChild('memberTabs', {static:true}) memberTabs? : TabsetComponent;
activeTab?:TabDirective;
member:Member = {} as Member;
galleryOptions : NgxGalleryOptions[]=[];
galleryImages : NgxGalleryImage[] =[];
messages : Message[]=[];

constructor(private memberService:MembersService, 
            private route:ActivatedRoute, 
            private messageService:MessageService,
            private toastr: ToastrService){}

ngOnInit(): void {
  //this.loadMember();
  this.route.data.subscribe({
    next: data=>{
      this.member = data['member']
    }
  });

  this.route.queryParams.subscribe({
    next: params =>{
      params['tab'] && this.selectTab(params['tab'])
    }
  })

  this.galleryOptions =[
    {
      width:'500px',
      height:'500px',
      imagePercent: 100,
      thumbnailsColumns :4,
      imageAnimation: NgxGalleryAnimation.Fade,
      preview:false
    }
  ]

  this.galleryImages = this.getImages();

 
}

getImages()
{
  if(!this.member) return[];
  const imageUrls =[];

  for(const photo of this.member.photos)
  {
    imageUrls.push({
      small:photo.url,
      medium: photo.url,
      big: photo.url
    });
  }

  return imageUrls;
}

/* loadMember(){

  var username = this.route.snapshot.paramMap.get('username')
  if(!username) return;

  this.memberService.getMember(username).subscribe({
    next: member => {
      this.member = member;
      this.galleryImages = this.getImages();
    }
  })
} */

loadMessages(){
  if(this.member){
    this.messageService.getMessageThread(this.member.userName).subscribe({
      next: messages => this.messages = messages
    });
  }
}

onTabActivated(data:TabDirective){
  this.activeTab = data;
  if(this.activeTab.heading === "Messages"){
      this.loadMessages();
  }
}

selectTab(heading: string){
  if(this.memberTabs){
    this.memberTabs.tabs.find(x=> x.heading === heading)!.active = true
  }
}

addLike(member:Member){
  this.memberService.addLike(member.userName).subscribe({
    next: () => this.toastr.success("Thanks for liking "+ member.fullName)
  })
}

}
