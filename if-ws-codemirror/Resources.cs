using IntelliFactory.WebSharper.Core;
using IntelliFactory.WebSharper;

namespace IntelliFactory.WebSharper.CodeMirror.Resources
{
  public class Css : Core.Resources.BaseResource
  {
	public Css() : base("CodeMirror.lib.codemirror.css") { }
  }

  [Attributes.Require(typeof(Css))]
  public class Js : Core.Resources.BaseResource
  {
	public Js() : base("CodeMirror.lib.codemirror.js") { }
  }
}

namespace IntelliFactory.WebSharper.CodeMirror.Resources.Modes
{
  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Clike : Core.Resources.BaseResource
  {
	public Clike() : base("CodeMirror.mode.clike.clike.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Clojure : Core.Resources.BaseResource
  {
	public Clojure() : base("CodeMirror.mode.clojure.clojure.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Coffeescript : Core.Resources.BaseResource
  {
	public Coffeescript() : base("CodeMirror.mode.coffeescript.coffeescript.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Css : Core.Resources.BaseResource
  {
	public Css() : base("CodeMirror.mode.css.css.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Diff : Core.Resources.BaseResource
  {
	public Diff() : base("CodeMirror.mode.diff.diff.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Ecl : Core.Resources.BaseResource
  {
	public Ecl() : base("CodeMirror.mode.ecl.ecl.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Erlang : Core.Resources.BaseResource
  {
	public Erlang() : base("CodeMirror.mode.erlang.erlang.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Gfm : Core.Resources.BaseResource
  {
	public Gfm() : base("CodeMirror.mode.gfm.gfm.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Go : Core.Resources.BaseResource
  {
	public Go() : base("CodeMirror.mode.go.go.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Groovy : Core.Resources.BaseResource
  {
	public Groovy() : base("CodeMirror.mode.groovy.groovy.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Haskell : Core.Resources.BaseResource
  {
	public Haskell() : base("CodeMirror.mode.haskell.haskell.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Htmlembedded : Core.Resources.BaseResource
  {
	public Htmlembedded() : base("CodeMirror.mode.htmlembedded.htmlembedded.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Htmlmixed : Core.Resources.BaseResource
  {
	public Htmlmixed() : base("CodeMirror.mode.htmlmixed.htmlmixed.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Javascript : Core.Resources.BaseResource
  {
	public Javascript() : base("CodeMirror.mode.javascript.javascript.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Jinja2 : Core.Resources.BaseResource
  {
	public Jinja2() : base("CodeMirror.mode.jinja2.jinja2.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Less : Core.Resources.BaseResource
  {
	public Less() : base("CodeMirror.mode.less.less.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Lua : Core.Resources.BaseResource
  {
	public Lua() : base("CodeMirror.mode.lua.lua.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Markdown : Core.Resources.BaseResource
  {
	public Markdown() : base("CodeMirror.mode.markdown.markdown.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Mysql : Core.Resources.BaseResource
  {
	public Mysql() : base("CodeMirror.mode.mysql.mysql.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Ntriples : Core.Resources.BaseResource
  {
	public Ntriples() : base("CodeMirror.mode.ntriples.ntriples.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Pascal : Core.Resources.BaseResource
  {
	public Pascal() : base("CodeMirror.mode.pascal.pascal.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Perl : Core.Resources.BaseResource
  {
	public Perl() : base("CodeMirror.mode.perl.perl.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Php : Core.Resources.BaseResource
  {
	public Php() : base("CodeMirror.mode.php.php.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Pig : Core.Resources.BaseResource
  {
	public Pig() : base("CodeMirror.mode.pig.pig.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Plsql : Core.Resources.BaseResource
  {
	public Plsql() : base("CodeMirror.mode.plsql.plsql.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Properties : Core.Resources.BaseResource
  {
	public Properties() : base("CodeMirror.mode.properties.properties.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Python : Core.Resources.BaseResource
  {
	public Python() : base("CodeMirror.mode.python.python.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class R : Core.Resources.BaseResource
  {
	public R() : base("CodeMirror.mode.r.r.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class RpmChanges : Core.Resources.BaseResource
  {
	public RpmChanges() : base("CodeMirror.mode.rpm.changes.changes.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class RpmSpec : Core.Resources.BaseResource
  {
	public RpmSpec() : base("CodeMirror.mode.rpm.spec.spec.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Rst : Core.Resources.BaseResource
  {
	public Rst() : base("CodeMirror.mode.rst.rst.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Ruby : Core.Resources.BaseResource
  {
	public Ruby() : base("CodeMirror.mode.ruby.ruby.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Rust : Core.Resources.BaseResource
  {
	public Rust() : base("CodeMirror.mode.rust.rust.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Scheme : Core.Resources.BaseResource
  {
	public Scheme() : base("CodeMirror.mode.scheme.scheme.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Shell : Core.Resources.BaseResource
  {
	public Shell() : base("CodeMirror.mode.shell.shell.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Smalltalk : Core.Resources.BaseResource
  {
	public Smalltalk() : base("CodeMirror.mode.smalltalk.smalltalk.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Smarty : Core.Resources.BaseResource
  {
	public Smarty() : base("CodeMirror.mode.smarty.smarty.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Sparql : Core.Resources.BaseResource
  {
	public Sparql() : base("CodeMirror.mode.sparql.sparql.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Stex : Core.Resources.BaseResource
  {
	public Stex() : base("CodeMirror.mode.stex.stex.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Tiddlywiki : Core.Resources.BaseResource
  {
	public Tiddlywiki() : base("CodeMirror.mode.tiddlywiki.tiddlywiki.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Tiki : Core.Resources.BaseResource
  {
	public Tiki() : base("CodeMirror.mode.tiki.tiki.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Vbscript : Core.Resources.BaseResource
  {
	public Vbscript() : base("CodeMirror.mode.vbscript.vbscript.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Velocity : Core.Resources.BaseResource
  {
	public Velocity() : base("CodeMirror.mode.velocity.velocity.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Verilog : Core.Resources.BaseResource
  {
	public Verilog() : base("CodeMirror.mode.verilog.verilog.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Xml : Core.Resources.BaseResource
  {
	public Xml() : base("CodeMirror.mode.xml.xml.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Xquery : Core.Resources.BaseResource
  {
	public Xquery() : base("CodeMirror.mode.xquery.xquery.js") { }
  }

  [Attributes.Require(typeof(IntelliFactory.WebSharper.CodeMirror.Resources.Js))]
  public class Yaml : Core.Resources.BaseResource
  {
	public Yaml() : base("CodeMirror.mode.yaml.yaml.js") { }
  }
}
