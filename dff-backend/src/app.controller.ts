import { Body, Controller, Get, Post } from '@nestjs/common';
import { CheckSignaturesDto } from './checkSignatures.dto';
import { CheckSignaturesResponseDto } from './checkSignaturesResponse.dto';

@Controller('sigs')
export class AppController {
  signature1Storage: Map<string, string> = new Map<string, string>();
  signature2Storage: Map<string, string> = new Map<string, string>();
  signature3Storage: Map<string, string> = new Map<string, string>();

  constructor() {}

  @Post()
  async checkAndStoreSignatures(
    @Body() requestDto: CheckSignaturesDto,
  ): Promise<CheckSignaturesResponseDto> {

    let response = new CheckSignaturesResponseDto();
    if (this.signature1Storage.has(requestDto.signature1))
    {
      response.isDuplicate = true;
      response.duplicateSignature1 = requestDto.signature1;
      response.from = this.signature1Storage.get(requestDto.signature1);
    }

    if (this.signature2Storage.has(requestDto.signature2))
    {
      response.isDuplicate = true;
      response.duplicateSignature2 = requestDto.signature2;
      response.from = this.signature2Storage.get(requestDto.signature2);
    }

    if (this.signature3Storage.has(requestDto.signature3))
    {
      response.isDuplicate = true;
      response.duplicateSignature3 = requestDto.signature3;
      response.from = this.signature3Storage.get(requestDto.signature3);
    }

    if (response.isDuplicate) {
      return response;
    }

    this.signature1Storage.set(requestDto.signature1, requestDto.from);
    this.signature2Storage.set(requestDto.signature2, requestDto.from);
    this.signature3Storage.set(requestDto.signature3, requestDto.from);

    response.isDuplicate = false;
    return response;
  }
}
