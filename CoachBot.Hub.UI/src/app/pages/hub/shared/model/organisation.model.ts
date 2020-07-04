import { AssetImage } from '@shared/models/asset-image.model';

export class Organisation {
    id: number;
    name: string;
    acronym: string;
    logoImageId: number;
    logoImage: AssetImage;
    brandColour: string;
    updatedDate?: Date;
    createdDate?: Date;
}
