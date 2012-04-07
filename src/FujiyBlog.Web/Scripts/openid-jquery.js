var providers, openid;
(function (e) {
    openid = { version: "1.3", demo: !1, demo_text: null, cookie_expires: 180, cookie_name: "openid_provider", cookie_path: "/", img_path: "images/", locale: null, sprite: null, signin_text: null, all_small: !1, no_sprite: !1, image_title: "{provider}", input_id: null, provider_url: null, provider_id: null, init: function (a) {
        providers = e.extend({}, providers_large, providers_small); var b = e("#openid_btns"); this.input_id = a; e("#openid_choice").show(); e("#openid_input_area").empty(); a = 0; for (id in providers_large) box = this.getBoxHTML(id,
providers_large[id], this.all_small ? "small" : "large", a++), b.append(box); if (providers_small) for (id in b.append("<br/>"), providers_small) box = this.getBoxHTML(id, providers_small[id], "small", a++), b.append(box); e("#openid_form").submit(this.submit); (b = this.readCookie()) && this.signin(b, !0)
    }, getBoxHTML: function (a, b, c, d) {
        return this.no_sprite ? '<a title="' + this.image_title.replace("{provider}", b.name) + '" href="javascript:openid.signin(\'' + a + '\');" style="background: #FFF url(' + this.img_path + "../images." + c + "/" +
a + ("small" == c ? ".ico.gif" : ".gif") + ') no-repeat center center" class="' + a + " openid_" + c + '_btn"></a>' : '<a title="' + this.image_title.replace("{provider}", b.name) + '" href="javascript:openid.signin(\'' + a + '\');" style="background: #FFF url(' + this.img_path + "openid-providers-" + this.sprite + ".png); background-position: " + ("small" == c ? 24 * -d : 100 * -d) + "px " + ("small" == c ? -60 : 0) + 'px" class="' + a + " openid_" + c + '_btn"></a>'
    }, signin: function (a, b) {
        var c = providers[a]; c && (this.highlight(a), this.setCookie(a), this.provider_id =
a, this.provider_url = c.url, c.label ? this.useInputBox(c) : (e("#openid_input_area").empty(), b || e("#openid_form").submit()))
    }, submit: function () { var a = openid.provider_url; a && (a = a.replace("{username}", e("#openid_username").val()), openid.setOpenIdUrl(a)); return openid.demo ? (alert(openid.demo_text + "\r\n" + document.getElementById(openid.input_id).value), !1) : 0 == a.indexOf("javascript:") ? (a = a.substr(11), eval(a), !1) : !0 }, setOpenIdUrl: function (a) {
        var b = document.getElementById(this.input_id); null != b ? b.value = a : e("#openid_form").append('<input type="hidden" id="' +
this.input_id + '" name="' + this.input_id + '" value="' + a + '"/>')
    }, highlight: function (a) { var b = e("#openid_highlight"); b && b.replaceWith(e("#openid_highlight a")[0]); e("." + a).wrap('<div id="openid_highlight"></div>') }, setCookie: function (a) { var b = new Date; b.setTime(b.getTime() + 864E5 * this.cookie_expires); b = "; expires=" + b.toGMTString(); document.cookie = this.cookie_name + "=" + a + b + "; path=" + this.cookie_path }, readCookie: function () {
        for (var a = this.cookie_name + "=", b = document.cookie.split(";"), c = 0; c < b.length; c++) {
            for (var d =
b[c]; " " == d.charAt(0); ) d = d.substring(1, d.length); if (0 == d.indexOf(a)) return d.substring(a.length, d.length)
        } return null
    }, useInputBox: function (a) {
        var b = e("#openid_input_area"), c = "", d = "openid_username", f = "", g = a.label, h = ""; g && (c = "<p>" + g + "</p>"); "OpenID" == a.name && (d = this.input_id, f = "http://", h = "background: #FFF url(" + this.img_path + "openid-inputicon.gif) no-repeat scroll 0 50%; padding-left:18px;"); c += '<input id="' + d + '" type="text" style="' + h + '" name="' + d + '" value="' + f + '" />'; b.empty(); b.append(c); e("#" +
d).focus()
    }, setDemoMode: function (a) { this.demo = a } 
    }
})(jQuery);
var providers_large = { google: { name: "Google", url: "https://www.google.com/accounts/o8/id" }, yahoo: { name: "Yahoo", url: "http://me.yahoo.com/" }, aol: { name: "AOL", label: "Enter your AOL screenname.", url: "http://openid.aol.com/{username}" }, myopenid: { name: "MyOpenID", label: "Enter your MyOpenID username.", url: "http://{username}.myopenid.com/" }, openid: { name: "OpenID", label: "Enter your OpenID.", url: null} }, providers_small = { livejournal: { name: "LiveJournal", label: "Enter your Livejournal username.", url: "http://{username}.livejournal.com/" },
    wordpress: { name: "Wordpress", label: "Enter your Wordpress.com username.", url: "http://{username}.wordpress.com/" }, blogger: { name: "Blogger", label: "Your Blogger account", url: "http://{username}.blogspot.com/" }, verisign: { name: "Verisign", label: "Your Verisign username", url: "http://{username}.pip.verisignlabs.com/" }, claimid: { name: "ClaimID", label: "Your ClaimID username", url: "http://claimid.com/{username}" }, clickpass: { name: "ClickPass", label: "Enter your ClickPass username", url: "http://clickpass.com/public/{username}" },
    google_profile: { name: "Google Profile", label: "Enter your Google Profile username", url: "http://www.google.com/profiles/{username}"}
}; openid.locale = "en"; openid.sprite = "en"; openid.demo_text = "In client demo mode. Normally would have submitted OpenID:"; openid.signin_text = "Sign-In"; openid.image_title = "log in with {provider}";