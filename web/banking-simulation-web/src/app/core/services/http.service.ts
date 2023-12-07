import { HttpClient } from "@angular/common/http";

export class HttpService {
    baseUrl: string = "https://localhost:7117";

    constructor(httpClient: HttpClient) { }
}