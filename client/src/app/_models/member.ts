import { Photo } from "./photo";

export interface Member {
    id: number;
    username: string;
    photoUrl: string;
    age: number;
    created: Date;
    lastActive: Date;
    knownAs: string;
    gender: string;
    introduction: string;
    lookingFor: string;
    interests?: any;
    city: string;
    country: string;
    photos: Photo[];
}
