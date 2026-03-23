var textarea = document.querySelector('textarea[name="SolutionCode"]');
$.validator.setDefaults({
    ignore: ":hidden:not(textarea)"
});
var currentTheme = localStorage.getItem('theme') || 'light';
var editor = CodeMirror.fromTextArea(textarea, {
    lineNumbers: true,        
    mode: "text/x-csharp",
    theme: currentTheme === 'dark' ? "dracula" : "eclipse",
    indentUnit: 4,
    tabSize: 4,
    viewportMargin: Infinity
});

var languageSelect = document.querySelector("select[name='Language']");
languageSelect.addEventListener("change", function () {
    var lang = this.value;
    if (lang === "CSharp") editor.setOption("mode", "text/x-csharp");
    else if (lang === "JavaScript") editor.setOption("mode", "javascript");
    else if (lang === "Python") editor.setOption("mode", "python");
    else if (lang === "Java") editor.setOption("mode", "text/x-java");
    else editor.setOption("mode", "text/plain");
});

function updateTheme() {
    var currentTheme = localStorage.getItem('theme') || 'light';
    editor.setOption('theme', currentTheme === 'dark' ? 'dracula' : 'eclipse');
}
document.addEventListener("themeChanged", (e) => {
    const isDark = e.detail.isDark;
    editor.setOption("theme", isDark ? "dracula" : "eclipse");
});

$('form[method="post"]').on('submit', () => {
    editor.save();
    $("textarea[name='SolutionCode']").valid();
})