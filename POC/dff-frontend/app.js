const form = document.getElementById("invoice_data");

const btnGenSig = document.getElementById("generate_signatures");
const btnSendSig = document.getElementById("send_signatures");
const pSignature1 = document.getElementById("potpis_1");
const pSignature2 = document.getElementById("potpis_2");
const pSignature3 = document.getElementById("potpis_3");
const result = document.getElementById("result");

btnGenSig.addEventListener("click", function (event) {
	event.preventDefault();
    result.textContent = '';

    const matBrojDob = form.elements["mat_broj_dobavljac"].value;
    const matBrojKupac = form.elements["mat_broj_kupac"].value;
    const brojFakture = form.elements["broj_fakture"].value;
    const datumIzdavanja = form.elements["datum_izdavanja"].value;
    const datumValute = form.elements["datum_valute"].value;
    const iznos = form.elements["iznos"].value;

    const signature1 = hashCode(matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute + iznos);
    pSignature1.textContent = signature1;
    pSignature1.className = '';

    const signature2 = hashCode(matBrojDob + matBrojKupac + datumIzdavanja + datumValute + iznos);
    pSignature2.textContent = signature2;
    pSignature2.className = '';

    const signature3 = hashCode(matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute);
    pSignature3.textContent = signature3;
    pSignature3.className = '';
});

btnSendSig.addEventListener("click", function (event) {
	event.preventDefault();

    const requestHeaders = new Headers();
    requestHeaders.append('Content-Type', 'application/json');

    const requestBody = {
        from: "finspot",
        signature1: pSignature1.textContent,
        signature2: pSignature2.textContent,
        signature3: pSignature3.textContent
    }

    fetch('http://127.0.0.1:3000/sigs', {
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

        if (data.isDuplicate) {
            result.textContent = "Faktura je bila predmet faktoringa.";
            result.className = "signature-alert";

            if (data.duplicateSignature1 == pSignature1.textContent) {
                pSignature1.className = "signature-alert";
            }

            if (data.duplicateSignature2 == pSignature2.textContent) {
                pSignature2.className = "signature-alert";
            }

            if (data.duplicateSignature3 == pSignature3.textContent) {
                pSignature3.className = "signature-alert";
            }

        } else {
            result.textContent = "Faktura nije bila predmet faktoringa.";
            result.className = "signature-ok";
        }

    }).catch(function (err) {
        result.textContent = "Došlo je do greške!";
        result.className = "signature-alert";
        console.warn('Something went wrong.', err);
    });
});

 function hashCode(str) {
    const seed = 0;
    let h1 = 0xdeadbeef ^ seed,
        h2 = 0x41c6ce57 ^ seed;
    for (let i = 0, ch; i < str.length; i++) {
        ch = str.charCodeAt(i);
        h1 = Math.imul(h1 ^ ch, 2654435761);
        h2 = Math.imul(h2 ^ ch, 1597334677);
    }
    
    h1 = Math.imul(h1 ^ (h1 >>> 16), 2246822507) ^ Math.imul(h2 ^ (h2 >>> 13), 3266489909);
    h2 = Math.imul(h2 ^ (h2 >>> 16), 2246822507) ^ Math.imul(h1 ^ (h1 >>> 13), 3266489909);
    
    return 4294967296 * (2097151 & h2) + (h1 >>> 0);
}