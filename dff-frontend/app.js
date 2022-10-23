const form = document.getElementById("invoice_data");

const btnGenSig = document.getElementById("generate_signatures");
const btnSendSig = document.getElementById("send_signatures");

btnGenSig.addEventListener("click", function (event) {
	event.preventDefault();
    const matBrojDob = form.elements["mat_broj_dobavljac"].value;
    const matBrojKupac = form.elements["mat_broj_kupac"].value;
    const brojFakture = form.elements["broj_fakture"].value;
    const datumIzdavanja = form.elements["datum_izdavanja"].value;
    const datumValute = form.elements["datum_valute"].value;
    const iznos = form.elements["iznos"].value;


    console.log(hashCode(matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute + iznos));
    // TODO: Generate sigs and populate aside
});

btnSendSig.addEventListener("click", function (event) {
	event.preventDefault();
    console.log("TODO: fire away");
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