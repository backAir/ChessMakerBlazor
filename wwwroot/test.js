// function that returns hello + argument
function hello(name) {
    return "Hello " + name;
}

function copyToClipboard (text) {
    navigator.clipboard.writeText(text);
}

function playSound (text) {
    document.getElementById(text).play();
}