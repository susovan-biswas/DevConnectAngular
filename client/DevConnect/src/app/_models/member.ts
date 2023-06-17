import { Blog } from "./blog";
import { Photo } from "./photo";

export interface Member {
  id: number;
  userName: string;
  fullName: string;
  photoUrl: string;
  age: number;
  created: string;
  lastActive: string;
  introduction: string;
  skills: string;
  city: string;
  country: string;
  photos: Photo[];
  blogs: Blog[];
}