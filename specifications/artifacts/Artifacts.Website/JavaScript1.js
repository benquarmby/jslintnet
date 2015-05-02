function myFunction() {
    var x = "";

    var time = new Date().getHours();

    if (time < 20) {
        x = "Good day";
    }
    document.getElementById("demo").innerHTML = x;
}
