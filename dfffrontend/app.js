const authForm = document.getElementById("auth_data");
const invoiceForm = document.getElementById("invoice_data");

const btnGenSig = document.getElementById("generate_signatures");
const btnChkDups = document.getElementById("check_duplicates");
const btnSendSig = document.getElementById("send_signatures");
const pSignature1 = document.getElementById("potpis_1");
const pSignature2 = document.getElementById("potpis_2");
const pSignature3 = document.getElementById("potpis_3");
const pSignature4 = document.getElementById("potpis_4");
const result = document.getElementById("result");

btnGenSig.addEventListener("click", async function (event) {
	event.preventDefault();
    result.textContent = '';

    const matBrojDob = invoiceForm.elements["mat_broj_dobavljac"].value;
    const matBrojKupac = invoiceForm.elements["mat_broj_kupac"].value;
    const brojFakture = invoiceForm.elements["broj_fakture"].value;
    const datumIzdavanja = invoiceForm.elements["datum_izdavanja"].value;
    const datumValute = invoiceForm.elements["datum_valute"].value;
    const iznos = invoiceForm.elements["iznos"].value;
    const sefId = invoiceForm.elements["sef_id"].value;

    const signature1 = await hashCode(matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute + iznos);
    pSignature1.textContent = signature1;
    pSignature1.className = '';

    const signature2 = await hashCode(matBrojDob + matBrojKupac + datumIzdavanja + datumValute + iznos);
    pSignature2.textContent = signature2;
    pSignature2.className = '';

    const signature3 = await hashCode(matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute);
    pSignature3.textContent = signature3;
    pSignature3.className = '';

    const signature4 = await hashCode(sefId);
    pSignature4.textContent = signature4;
    pSignature4.className = '';
});

btnChkDups.addEventListener("click", async function (event) {
	event.preventDefault();

    const apiKey = authForm.elements["api_key"].value;

    const requestHeaders = new Headers();
    requestHeaders.append('Content-Type', 'application/json');
    requestHeaders.append('X-Factor-API-Key', apiKey.textContent);

    const signatureSet = {
        signature1: pSignature1.textContent,
        signature2: pSignature2.textContent,
        signature3: pSignature3.textContent,
        signature4: pSignature4.textContent,
    }

    const requestBody = {
        signatureSets: [signatureSet]
    }

    const response = await fetch('http://localhost:5296/api/Signatures/check', {
        headers: requestHeaders,
    	method: 'POST',
        body: JSON.stringify(requestBody),
    });

    if (!response.ok) {
        result.textContent = "Došlo je do greške!";
        result.className = "signature-alert";
        console.warn('Something went wrong.', await response.json());
        return;
    }

    const responseData = await response.json();
    console.log(JSON.stringify(responseData));

    // if (data.isDuplicate) {
    //     result.textContent = "Faktura je bila predmet faktoringa.";
    //     result.className = "signature-alert";

    //     if (data.duplicateSignature1 == pSignature1.textContent) {
    //         pSignature1.className = "signature-alert";
    //     }

    //     if (data.duplicateSignature2 == pSignature2.textContent) {
    //         pSignature2.className = "signature-alert";
    //     }

    //     if (data.duplicateSignature3 == pSignature3.textContent) {
    //         pSignature3.className = "signature-alert";
    //     }

    // } else {
    //     result.textContent = "Faktura nije bila predmet faktoringa.";
    //     result.className = "signature-ok";
    // }
});

btnSendSig.addEventListener("click", function (event) {
	event.preventDefault();

    const apiKey = authForm.elements["api_key"].value;

    const requestHeaders = new Headers();
    requestHeaders.append('Content-Type', 'application/json');
    requestHeaders.append('X-Factor-API-Key', apiKey.textContent);

    const signatureSet = {
        signature1: pSignature1.textContent,
        signature2: pSignature2.textContent,
        signature3: pSignature3.textContent,
        signature4: pSignature4.textContent,
    }

    const requestBody = {
        signatureSets: [signatureSet]
    }

    fetch('http://localhost:5296/api/Signatures/checkandstore', {
        headers: requestHeaders,
    	method: 'POST',
        body: JSON.stringify(requestBody),
    }).then(function (response) {
        if (response.ok) {
            return response.json();
        } else {
            return Promise.reject(response);
        }
    }).then(function (data) {

        console.log(JSON.stringify(data));

        // if (data.isDuplicate) {
        //     result.textContent = "Faktura je bila predmet faktoringa.";
        //     result.className = "signature-alert";

        //     if (data.duplicateSignature1 == pSignature1.textContent) {
        //         pSignature1.className = "signature-alert";
        //     }

        //     if (data.duplicateSignature2 == pSignature2.textContent) {
        //         pSignature2.className = "signature-alert";
        //     }

        //     if (data.duplicateSignature3 == pSignature3.textContent) {
        //         pSignature3.className = "signature-alert";
        //     }

        // } else {
        //     result.textContent = "Faktura nije bila predmet faktoringa.";
        //     result.className = "signature-ok";
        // }

    }).catch(function (err) {
        result.textContent = "Došlo je do greške!";
        result.className = "signature-alert";
    });
});

async function hashCode(str) {
    console.log(`Applying SHA-256 on the following string: ` + str)
    const textAsBuffer = new TextEncoder().encode(str);
    const hashBuffer = await window.crypto.subtle.digest('SHA-256', textAsBuffer);
    const hashArray = Array.from(new Uint8Array(hashBuffer))
    return hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
}