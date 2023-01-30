async function generateSignatures(matBrojDob, matBrojKupac, brojFakture, datumIzdavanja, datumValute, iznos, sefId) {
    let signature1;
    let signature2;
    let signature3;
    let signature4;

    const signature1Input = matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute + iznos;
    if (signature1Input !== undefined && signature1Input.length > 0) {
        signature1 = await hashCode(signature1Input);
    }

    const signature2Input = matBrojDob + matBrojKupac + datumIzdavanja + datumValute + iznos;
    if (signature2Input !== undefined && signature2Input.length > 0) {
        signature2 = await hashCode(signature2Input);
    }
    
    const signature3Input = matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute;
    if (signature3Input !== undefined && signature3Input.length > 0) {
        signature3 = await hashCode(signature3Input);
    }

    const signature4Input = sefId;
    if(signature4Input !== undefined && signature4Input.length > 0) {
        signature4 = await hashCode(signature4Input);
    }

    return { signature1: signature1, signature2: signature2, signature3: signature3, signature4: signature4 };
}

async function hashCode(str) {
    console.log(`Applying SHA-256 on the following string: ` + str)
    const textAsBuffer = new TextEncoder().encode(str);
    const hashBuffer = await window.crypto.subtle.digest('SHA-256', textAsBuffer);
    const hashArray = Array.from(new Uint8Array(hashBuffer))
    return hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
}

function prepareReqBodyFromSignatureSets(signatureSets) {
    const requestBody = {
        signaturesSets: signatureSets
    }

    return requestBody;
}