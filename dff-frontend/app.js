const form = document.getElementById("invoice_data");

const btnGenSig = document.getElementById("generate_signatures");
const btnSendSig = document.getElementById("send_signatures");
const pSignature1 = document.getElementById("potpis_1");
const pSignature2 = document.getElementById("potpis_2");
const pSignature3 = document.getElementById("potpis_3");
const result = document.getElementById("result");
let isDuplicate = false;
result.hidden = !isDuplicate;

btnGenSig.addEventListener("click", function (event) {
	event.preventDefault();
    result.hidden = true;

    const matBrojDob = form.elements["mat_broj_dobavljac"].value;
    const matBrojKupac = form.elements["mat_broj_kupac"].value;
    const brojFakture = form.elements["broj_fakture"].value;
    const datumIzdavanja = form.elements["datum_izdavanja"].value;
    const datumValute = form.elements["datum_valute"].value;
    const iznos = form.elements["iznos"].value;

    const signature1 = hashCode(matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute + iznos);
    pSignature1.textContent = signature1;

    const signature2 = hashCode(matBrojDob + matBrojKupac + datumIzdavanja + datumValute + iznos);
    pSignature2.textContent = signature2;

    const signature3 = hashCode(matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute);
    pSignature3.textContent = signature3;
});

btnSendSig.addEventListener("click", function (event) {
	event.preventDefault();

    fetch('http://127.0.0.1:3000/sigs', {
    	method: 'POST',
        body: {
            from: "finspot",
            signature1: pSignature1.textContent,
            signature2: pSignature2.textContent,
            signature3: pSignature3.textContent
        }
    }).then(function (response) {
        // The API call was successful!
        if (response.ok) {
            return response.json();
        } else {
            return Promise.reject(response);
        }
    }).then(function (data) {
        // This is the JSON from our response
        isDuplicate = data.isDuplicate;
        console.log(data);
    }).catch(function (err) {
        // There was an error
        console.warn('Something went wrong.', err);
    });

    
    result.hidden = !isDuplicate;

    if (isDuplicate) {
        result.textContent = "Faktura je duplikat!";
        result.className = "toast-failure";
    } else {
        result.textContent = "";
        result.className = "toast-success";
    }
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