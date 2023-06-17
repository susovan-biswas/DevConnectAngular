import { Post } from "./post";

export interface Blog {
  id: number;
  name?: any;
  description?: any;
  url: string;
  posts: Post[];
}
