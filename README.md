<div id="Ad1" data-type="ad" data-publisher="" data-zone="ron" style="width: 300px; height: 250px; margin: 0px auto;" data-site="C-languagetranslator" data-format="300x250"> 
 
<script type='text/javascript'>
function _dmBootstrap(file) {
    var _dma = document.createElement('script');
    _dma.type = 'text/javascript';
    _dma.async = true;
    _dma.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + file;
    (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(_dma);
}
function _dmFollowup(file) { if (typeof DMAds === 'undefined') _dmBootstrap('cdn2.DeveloperMedia.com/a.min.js'); }
(function () { _dmBootstrap('cdn1.DeveloperMedia.com/a.min.js'); setTimeout(_dmFollowup, 2000); })();
</script>
</div>               
<div class="wikidoc">
<h1><b>Project Description</b></h1>
Extends OpenFileDialog and SaveFileDialog Using Windows Forms or WPF<br>
<br>
<br>
<b>**This project contains the source code for the articles published on codeproject **</b><br>
<br>
If you used WinForms or WPF, chances are that at some point you wanted to extend the OpenFileDialog or SaveFileDialog, but you gave up because there is no easy way to do it, especially if you wanted to add some new graphical elements. The source code provided
 makes customization of these dialogs very easy, and shows how to call it for a quick ramp up.<br>
I’ve included the equivalent VB.NET code in the downloadable zip file for the VB folks.<br>
<h1><b>How to use it</b></h1>
<h2>For Windows Forms</h2>
<img src="http://www.codeproject.com/KB/dialog/CustomizeFileDialog/saveas.jpg"><br>
<br>
To start using it, you can drop the code in your project or just add a reference to the FileDlgExtenders.dll assembly or to FileDlgExtenders project. If you choose the latter, build the solution before you move forward, because you need the base class at design
 time. To make things as easy as possible, select &#39;Add User Control&#39; to your project, than pick &#39;Inherited User Control&#39; and finally select FileDialogControlBase from the list.
<br>
<h2>For WPF</h2>
<img src="http://www.codeproject.com/KB/dialog/WPFCustomFileDialog/SelectFile.PNG"><br>
<br>
<a href="http://www.codeproject.com/Articles/42008/Extend-OpenFileDialog-and-SaveFileDialog-Using-WPF#heading0005">Implement IWindowExt</a> or
<a href="http://www.codeproject.com/Articles/42008/Extend-OpenFileDialog-and-SaveFileDialog-Using-WPF#heading0007">
Inherit from the WindowAddOnBase or the ControlAddOnBase Class</a><br>
<h1><b>More about it</b></h1>
A lot of additional information can be found on codeproject for <a href="http://www.codeproject.com/Articles/42008/Extend-OpenFileDialog-and-SaveFileDialog-Using-WPF">
WPF</a> and <a href="http://www.codeproject.com/Articles/19566/Extend-OpenFileDialog-and-SaveFileDialog-the-easy">
Windows Forms</a> .</div>
