var $estr = function() { return js.Boot.__string_rec(this,''); };
function $extend(from, fields) {
	function inherit() {}; inherit.prototype = from; var proto = new inherit();
	for (var name in fields) proto[name] = fields[name];
	return proto;
}
var Selection = function() { }
Selection.__name__ = true;
Selection.prototype = {
	__class__: Selection
}
var MessagePortArray = function() { }
MessagePortArray.__name__ = true;
var MessagePort = function() { }
MessagePort.__name__ = true;
MessagePort.prototype = {
	__class__: MessagePort
}
var Hash = function() {
	this.h = { };
};
Hash.__name__ = true;
Hash.prototype = {
	toString: function() {
		var s = new StringBuf();
		s.b += Std.string("{");
		var it = this.keys();
		while( it.hasNext() ) {
			var i = it.next();
			s.b += Std.string(i);
			s.b += Std.string(" => ");
			s.b += Std.string(Std.string(this.get(i)));
			if(it.hasNext()) s.b += Std.string(", ");
		}
		s.b += Std.string("}");
		return s.b;
	}
	,iterator: function() {
		return { ref : this.h, it : this.keys(), hasNext : function() {
			return this.it.hasNext();
		}, next : function() {
			var i = this.it.next();
			return this.ref["$" + i];
		}};
	}
	,keys: function() {
		var a = [];
		for( var key in this.h ) {
		if(this.h.hasOwnProperty(key)) a.push(key.substr(1));
		}
		return HxOverrides.iter(a);
	}
	,remove: function(key) {
		key = "$" + key;
		if(!this.h.hasOwnProperty(key)) return false;
		delete(this.h[key]);
		return true;
	}
	,exists: function(key) {
		return this.h.hasOwnProperty("$" + key);
	}
	,get: function(key) {
		return this.h["$" + key];
	}
	,set: function(key,value) {
		this.h["$" + key] = value;
	}
	,__class__: Hash
}
var HxOverrides = function() { }
HxOverrides.__name__ = true;
HxOverrides.dateStr = function(date) {
	var m = date.getMonth() + 1;
	var d = date.getDate();
	var h = date.getHours();
	var mi = date.getMinutes();
	var s = date.getSeconds();
	return date.getFullYear() + "-" + (m < 10?"0" + m:"" + m) + "-" + (d < 10?"0" + d:"" + d) + " " + (h < 10?"0" + h:"" + h) + ":" + (mi < 10?"0" + mi:"" + mi) + ":" + (s < 10?"0" + s:"" + s);
}
HxOverrides.strDate = function(s) {
	switch(s.length) {
	case 8:
		var k = s.split(":");
		var d = new Date();
		d.setTime(0);
		d.setUTCHours(k[0]);
		d.setUTCMinutes(k[1]);
		d.setUTCSeconds(k[2]);
		return d;
	case 10:
		var k = s.split("-");
		return new Date(k[0],k[1] - 1,k[2],0,0,0);
	case 19:
		var k = s.split(" ");
		var y = k[0].split("-");
		var t = k[1].split(":");
		return new Date(y[0],y[1] - 1,y[2],t[0],t[1],t[2]);
	default:
		throw "Invalid date format : " + s;
	}
}
HxOverrides.cca = function(s,index) {
	var x = s.charCodeAt(index);
	if(x != x) return undefined;
	return x;
}
HxOverrides.substr = function(s,pos,len) {
	if(pos != null && pos != 0 && len != null && len < 0) return "";
	if(len == null) len = s.length;
	if(pos < 0) {
		pos = s.length + pos;
		if(pos < 0) pos = 0;
	} else if(len < 0) len = s.length + len - pos;
	return s.substr(pos,len);
}
HxOverrides.remove = function(a,obj) {
	var i = 0;
	var l = a.length;
	while(i < l) {
		if(a[i] == obj) {
			a.splice(i,1);
			return true;
		}
		i++;
	}
	return false;
}
HxOverrides.iter = function(a) {
	return { cur : 0, arr : a, hasNext : function() {
		return this.cur < this.arr.length;
	}, next : function() {
		return this.arr[this.cur++];
	}};
}
var IntHash = function() {
	this.h = { };
};
IntHash.__name__ = true;
IntHash.prototype = {
	toString: function() {
		var s = new StringBuf();
		s.b += Std.string("{");
		var it = this.keys();
		while( it.hasNext() ) {
			var i = it.next();
			s.b += Std.string(i);
			s.b += Std.string(" => ");
			s.b += Std.string(Std.string(this.get(i)));
			if(it.hasNext()) s.b += Std.string(", ");
		}
		s.b += Std.string("}");
		return s.b;
	}
	,iterator: function() {
		return { ref : this.h, it : this.keys(), hasNext : function() {
			return this.it.hasNext();
		}, next : function() {
			var i = this.it.next();
			return this.ref[i];
		}};
	}
	,keys: function() {
		var a = [];
		for( var key in this.h ) {
		if(this.h.hasOwnProperty(key)) a.push(key | 0);
		}
		return HxOverrides.iter(a);
	}
	,remove: function(key) {
		if(!this.h.hasOwnProperty(key)) return false;
		delete(this.h[key]);
		return true;
	}
	,exists: function(key) {
		return this.h.hasOwnProperty(key);
	}
	,get: function(key) {
		return this.h[key];
	}
	,set: function(key,value) {
		this.h[key] = value;
	}
	,__class__: IntHash
}
var IntIter = function(min,max) {
	this.min = min;
	this.max = max;
};
IntIter.__name__ = true;
IntIter.prototype = {
	next: function() {
		return this.min++;
	}
	,hasNext: function() {
		return this.min < this.max;
	}
	,__class__: IntIter
}
var Main = function() { }
Main.__name__ = true;
Main.main = function() {
}
var Std = function() { }
Std.__name__ = true;
Std["is"] = function(v,t) {
	return js.Boot.__instanceof(v,t);
}
Std.string = function(s) {
	return js.Boot.__string_rec(s,"");
}
Std["int"] = function(x) {
	return x | 0;
}
Std.parseInt = function(x) {
	var v = parseInt(x,10);
	if(v == 0 && (HxOverrides.cca(x,1) == 120 || HxOverrides.cca(x,1) == 88)) v = parseInt(x);
	if(isNaN(v)) return null;
	return v;
}
Std.parseFloat = function(x) {
	return parseFloat(x);
}
Std.random = function(x) {
	return Math.floor(Math.random() * x);
}
var StringBuf = function() {
	this.b = "";
};
StringBuf.__name__ = true;
StringBuf.prototype = {
	toString: function() {
		return this.b;
	}
	,addSub: function(s,pos,len) {
		this.b += HxOverrides.substr(s,pos,len);
	}
	,addChar: function(c) {
		this.b += String.fromCharCode(c);
	}
	,add: function(x) {
		this.b += Std.string(x);
	}
	,__class__: StringBuf
}
var StringTools = function() { }
StringTools.__name__ = true;
StringTools.urlEncode = function(s) {
	return encodeURIComponent(s);
}
StringTools.urlDecode = function(s) {
	return decodeURIComponent(s.split("+").join(" "));
}
StringTools.htmlEscape = function(s) {
	return s.split("&").join("&amp;").split("<").join("&lt;").split(">").join("&gt;");
}
StringTools.htmlUnescape = function(s) {
	return s.split("&gt;").join(">").split("&lt;").join("<").split("&amp;").join("&");
}
StringTools.startsWith = function(s,start) {
	return s.length >= start.length && HxOverrides.substr(s,0,start.length) == start;
}
StringTools.endsWith = function(s,end) {
	var elen = end.length;
	var slen = s.length;
	return slen >= elen && HxOverrides.substr(s,slen - elen,elen) == end;
}
StringTools.isSpace = function(s,pos) {
	var c = HxOverrides.cca(s,pos);
	return c >= 9 && c <= 13 || c == 32;
}
StringTools.ltrim = function(s) {
	var l = s.length;
	var r = 0;
	while(r < l && StringTools.isSpace(s,r)) r++;
	if(r > 0) return HxOverrides.substr(s,r,l - r); else return s;
}
StringTools.rtrim = function(s) {
	var l = s.length;
	var r = 0;
	while(r < l && StringTools.isSpace(s,l - r - 1)) r++;
	if(r > 0) return HxOverrides.substr(s,0,l - r); else return s;
}
StringTools.trim = function(s) {
	return StringTools.ltrim(StringTools.rtrim(s));
}
StringTools.rpad = function(s,c,l) {
	var sl = s.length;
	var cl = c.length;
	while(sl < l) if(l - sl < cl) {
		s += HxOverrides.substr(c,0,l - sl);
		sl = l;
	} else {
		s += c;
		sl += cl;
	}
	return s;
}
StringTools.lpad = function(s,c,l) {
	var ns = "";
	var sl = s.length;
	if(sl >= l) return s;
	var cl = c.length;
	while(sl < l) if(l - sl < cl) {
		ns += HxOverrides.substr(c,0,l - sl);
		sl = l;
	} else {
		ns += c;
		sl += cl;
	}
	return ns + s;
}
StringTools.replace = function(s,sub,by) {
	return s.split(sub).join(by);
}
StringTools.hex = function(n,digits) {
	var s = "";
	var hexChars = "0123456789ABCDEF";
	do {
		s = hexChars.charAt(n & 15) + s;
		n >>>= 4;
	} while(n > 0);
	if(digits != null) while(s.length < digits) s = "0" + s;
	return s;
}
StringTools.fastCodeAt = function(s,index) {
	return s.charCodeAt(index);
}
StringTools.isEOF = function(c) {
	return c != c;
}
var ddw = ddw || {}
ddw.Color = function(argb) {
	this.argb = argb;
	this.colorString = this.getColorString(argb);
};
ddw.Color.__name__ = true;
ddw.Color.fromAFlipRGB = function(afrgb) {
	var a = 255 - ((afrgb & -16777216) >>> 24) << 24;
	var rgb = afrgb & 16777215;
	return new ddw.Color(a + rgb);
}
ddw.Color.prototype = {
	getColorString: function(value) {
		var result;
		var a = (value & -16777216) >>> 24;
		var r = (value & 16711680) >>> 16;
		var g = (value & 65280) >>> 8;
		var b = value & 255;
		var vals = r + "," + g + "," + b;
		if(a < 255) result = "rgba(" + vals + "," + a + ")"; else result = "rgb(" + vals + ")";
		return result;
	}
	,getColorHex: function() {
		var result;
		var a = (this.argb & -16777216) >>> 24;
		var r = (this.argb & 16711680) >>> 16;
		var g = (this.argb & 65280) >>> 8;
		var b = this.argb & 255;
		result = StringTools.hex(a) + "" + StringTools.hex(r) + "" + StringTools.hex(g) + "" + StringTools.hex(b);
		return result;
	}
	,__class__: ddw.Color
}
ddw.Definition = function() {
};
ddw.Definition.__name__ = true;
ddw.Definition.prototype = {
	__class__: ddw.Definition
}
ddw.Fill = function() { }
ddw.Fill.__name__ = true;
ddw.Fill.prototype = {
	__class__: ddw.Fill
}
ddw.GradientFill = function(gradient) {
	this.gradient = gradient;
	this.isGradient = true;
	this.canvasFill = gradient;
};
ddw.GradientFill.__name__ = true;
ddw.GradientFill.__super__ = ddw.Fill;
ddw.GradientFill.prototype = $extend(ddw.Fill.prototype,{
	toString: function() {
		return "gradient";
	}
	,__class__: ddw.GradientFill
});
ddw.Image = function() {
	ddw.Definition.call(this);
	this.isTimeline = false;
	this.pendingDraws = new Array();
};
ddw.Image.__name__ = true;
ddw.Image.drawImage = function(img,metrics,vo) {
	var bnds = img.bounds;
	var offsetX = -bnds.x * metrics.scaleX;
	var offsetY = -bnds.y * metrics.scaleY;
	var divId = metrics.name == null || metrics.name == ""?"cv_" + metrics.instanceId:"cv_" + metrics.name;
	var cv = vo.createCanvas(divId,bnds.width * metrics.scaleX,bnds.height * metrics.scaleY);
	var g = cv.getContext("2d");
	img.draw(g,metrics);
}
ddw.Image.__super__ = ddw.Definition;
ddw.Image.prototype = $extend(ddw.Definition.prototype,{
	drawPending: function() {
		if(this.isLoaded) {
			var _g = 0, _g1 = this.pendingDraws;
			while(_g < _g1.length) {
				var idd = _g1[_g];
				++_g;
				this.draw(idd.g,idd.metrics);
			}
		}
		this.pendingDraws = new Array();
	}
	,draw: function(g,metrics) {
		if(this.isLoaded) g.drawImage(this.img,0,0,metrics.scaleX * this.bounds.width,metrics.scaleY * this.bounds.height); else {
			var idd = { g : g, metrics : metrics};
			this.pendingDraws.push(idd);
		}
	}
	,setPath: function(p) {
		var _g = this;
		this.isLoaded = false;
		this.path = p;
		var doc = js.Lib.document;
		this.img = doc.createElement("img");
		this.img.onload = function(_) {
			_g.isLoaded = true;
			_g.drawPending();
		};
		this.img.src = p;
	}
	,__class__: ddw.Image
});
ddw.Instance = function(defId,instId) {
	this.hasShear = false;
	this.hasRotation = false;
	this.hasScale = false;
	this.shear = 0;
	this.rotation = 0;
	this.scaleY = 1;
	this.scaleX = 1;
	this.y = 0;
	this.x = 0;
	this.definitionId = defId;
	if(instId == null) this.instanceId = ddw.Instance.instanceCounter++; else {
		this.instanceId = instId;
		if(ddw.Instance.instanceCounter <= instId) ddw.Instance.instanceCounter = instId + 1;
	}
};
ddw.Instance.__name__ = true;
ddw.Instance.drawInstance = function(inst,vo) {
	var divId = inst.name == null || inst.name == ""?"inst_" + inst.instanceId:inst.name;
	var div = vo.pushDiv(divId);
	var offsetX = 0;
	var offsetY = 0;
	var def = vo.definitions.get(inst.definitionId);
	if(def != null) {
		if(def.isTimeline) {
			var tl = js.Boot.__cast(def , ddw.Timeline);
			if(tl.instances.length > 1 || tl.instances.length == 1 && js.Boot.__instanceof(tl.instances[0],ddw.Timeline)) ddw.Timeline.drawTimeline(js.Boot.__cast(def , ddw.Timeline),vo); else {
				var symbol = js.Boot.__cast(vo.definitions.get(tl.instances[0].definitionId) , ddw.Symbol);
				var bnds = symbol.bounds;
				offsetX = -bnds.x * inst.scaleX;
				offsetY = -bnds.y * inst.scaleY;
				ddw.Symbol.drawSymbol(symbol,inst,vo);
			}
		} else if(js.Boot.__instanceof(def,ddw.Image)) {
			var img = js.Boot.__cast(vo.definitions.get(def.id) , ddw.Image);
			var bnds = img.bounds;
			offsetX = -bnds.x * inst.scaleX;
			offsetY = -bnds.y * inst.scaleY;
			ddw.Image.drawImage(img,inst,vo);
		} else ddw.Symbol.drawSymbol(js.Boot.__cast(def , ddw.Symbol),inst,vo);
	}
	vo.transformObject(div,inst,offsetX,offsetY);
	vo.popDiv();
}
ddw.Instance.prototype = {
	__class__: ddw.Instance
}
ddw.Path = function() {
	this.segments = new Array();
};
ddw.Path.__name__ = true;
ddw.Path.prototype = {
	__class__: ddw.Path
}
ddw.Rectangle = function(x,y,width,height) {
	this.x = x;
	this.y = y;
	this.width = width;
	this.height = height;
};
ddw.Rectangle.__name__ = true;
ddw.Rectangle.prototype = {
	__class__: ddw.Rectangle
}
ddw.Segment = function() {
	this.points = [];
};
ddw.Segment.__name__ = true;
ddw.Segment.prototype = {
	__class__: ddw.Segment
}
ddw.SegmentType = function() { }
ddw.SegmentType.__name__ = true;
ddw.Shape = function(strokeIndex,fillIndex,pathIndex) {
	this.strokeIndex = strokeIndex;
	this.fillIndex = fillIndex;
	this.pathIndex = pathIndex;
};
ddw.Shape.__name__ = true;
ddw.Shape.prototype = {
	__class__: ddw.Shape
}
ddw.SolidFill = function(color) {
	this.color = color;
	this.isGradient = false;
	this.canvasFill = color.colorString;
};
ddw.SolidFill.__name__ = true;
ddw.SolidFill.__super__ = ddw.Fill;
ddw.SolidFill.prototype = $extend(ddw.Fill.prototype,{
	toString: function() {
		return this.color.getColorHex();
	}
	,__class__: ddw.SolidFill
});
ddw.Stroke = function(color,lineWidth) {
	this.color = color;
	this.lineWidth = lineWidth;
};
ddw.Stroke.__name__ = true;
ddw.Stroke.prototype = {
	__class__: ddw.Stroke
}
ddw.Symbol = function() {
	ddw.Definition.call(this);
	this.isTimeline = false;
	this.paths = new Array();
	this.shapes = new Array();
};
ddw.Symbol.__name__ = true;
ddw.Symbol.drawSymbol = function(symbol,metrics,vo) {
	var bnds = symbol.bounds;
	var offsetX = -bnds.x * metrics.scaleX;
	var offsetY = -bnds.y * metrics.scaleY;
	var divId = metrics.name == null || metrics.name == ""?"cv_" + metrics.instanceId:"cv_" + metrics.name;
	var cv = vo.createCanvas(divId,bnds.width * metrics.scaleX,bnds.height * metrics.scaleY);
	var g = cv.getContext("2d");
	g.translate(offsetX,offsetY);
	if(metrics.hasScale) g.scale(metrics.scaleX,metrics.scaleY);
	var _g = 0, _g1 = symbol.shapes;
	while(_g < _g1.length) {
		var shape = _g1[_g];
		++_g;
		g.fillStyle = vo.fills[shape.fillIndex].canvasFill;
		g.lineWidth = vo.strokes[shape.strokeIndex].lineWidth;
		g.strokeStyle = vo.strokes[shape.strokeIndex].color.colorString;
		var path = symbol.paths[shape.pathIndex];
		g.beginPath();
		var _g2 = 0, _g3 = path.segments;
		while(_g2 < _g3.length) {
			var seg = _g3[_g2];
			++_g2;
			switch(seg.segmentType) {
			case 0:
				g.moveTo(seg.points[0],seg.points[1]);
				break;
			case 1:
				g.lineTo(seg.points[0],seg.points[1]);
				break;
			case 2:
				g.quadraticCurveTo(seg.points[0],seg.points[1],seg.points[2],seg.points[3]);
				break;
			case 3:
				g.bezierCurveTo(seg.points[0],seg.points[1],seg.points[2],seg.points[3],seg.points[4],seg.points[5]);
				break;
			}
		}
		if(shape.fillIndex > 0) g.fill();
		if(shape.strokeIndex > 0) g.stroke();
	}
}
ddw.Symbol.__super__ = ddw.Definition;
ddw.Symbol.prototype = $extend(ddw.Definition.prototype,{
	__class__: ddw.Symbol
});
ddw.Timeline = function() {
	ddw.Definition.call(this);
	this.isTimeline = true;
	this.instances = new Array();
};
ddw.Timeline.__name__ = true;
ddw.Timeline.drawTimeline = function(tl,vo) {
	var _g = 0, _g1 = tl.instances;
	while(_g < _g1.length) {
		var inst = _g1[_g];
		++_g;
		ddw.Instance.drawInstance(inst,vo);
	}
}
ddw.Timeline.__super__ = ddw.Definition;
ddw.Timeline.prototype = $extend(ddw.Definition.prototype,{
	__class__: ddw.Timeline
});
ddw.VexDrawBinaryReader = function(path,vo,onParseComplete) {
	this.idBitCount = 16;
	this.twips = 32;
	var _g = this;
	this.maskArray = [0,1,3,7,15,31,63,127,255,511,1023,2047,4095,8191,16383,32767,65535,131071,262143,524287,1048575,2097151,4194303,8388607,16777215,33554431,67108863,134217727,268435455,536870911,1073741823,2147483647,-1];
	var xhr = new XMLHttpRequest();
	xhr.open("GET",path,true);
	xhr.responseType = "arraybuffer";
	xhr.onload = function(e) {
		if(xhr.readyState == 4) {
			_g.data = new Uint8Array(xhr.response);
			_g.parseTags(vo);
			if(onParseComplete != null) onParseComplete();
		}
	};
	xhr.send();
};
ddw.VexDrawBinaryReader.__name__ = true;
ddw.VexDrawBinaryReader.prototype = {
	readNBits: function(nBits,result) {
		if(result == null) result = 0;
		var addingVal;
		var dif;
		var mask;
		while(nBits > 0) if(this.bit > nBits) {
			dif = this.bit - nBits;
			mask = this.maskArray[nBits] << dif;
			addingVal = (this.data[this.index] & mask) >>> dif;
			result = (result << nBits) + addingVal;
			this.bit -= nBits;
			nBits = 0;
		} else {
			mask = this.maskArray[this.bit];
			addingVal = this.data[this.index++] & mask;
			result = (result << this.bit) + addingVal;
			nBits -= this.bit;
			this.bit = 8;
		}
		return result;
	}
	,readNBitInt: function(nBits) {
		var result;
		var bitMask = Math.pow(2,this.bit - 1) | 0;
		if((this.data[this.index] & bitMask) != 0) result = this.readNBits(nBits,-1); else result = this.readNBits(nBits);
		return result;
	}
	,readNBitRect: function() {
		var nBits = this.readNBitCount();
		var result = new ddw.Rectangle(this.readNBitInt(nBits) / this.twips,this.readNBitInt(nBits) / this.twips,this.readNBitInt(nBits) / this.twips,this.readNBitInt(nBits) / this.twips);
		return result;
	}
	,readNBitCount: function() {
		var result = this.readNBits(5);
		return result + 2;
	}
	,readBit: function() {
		return this.readNBits(1) == 1?true:false;
	}
	,readByte: function() {
		return this.data[this.index++];
	}
	,flushBits: function() {
		if(this.bit != 8) {
			this.bit = 8;
			this.index++;
		}
		if(this.index % 4 != 0) this.index += 4 - this.index % 4;
	}
	,parseStrokes: function(vo) {
		this.strokeIndexNBits = this.readNBits(8);
		var colorNBits = this.readNBitCount();
		var lineWidthNBits = this.readNBitCount();
		var count = this.readNBits(11);
		var _g = 0;
		while(_g < count) {
			var i = _g++;
			var col = ddw.Color.fromAFlipRGB(this.readNBits(colorNBits));
			var lw = this.readNBits(lineWidthNBits) / this.twips;
			var stroke = new ddw.Stroke(col,lw);
			vo.strokes.push(stroke);
		}
		this.flushBits();
	}
	,parseSolidFills: function(vo) {
		this.fillIndexNBits = this.readNBits(8);
		var nBits = this.readNBitCount();
		var count = this.readNBits(11);
		var _g = 0;
		while(_g < count) {
			var i = _g++;
			var color = ddw.Color.fromAFlipRGB(this.readNBits(nBits));
			var fill = new ddw.SolidFill(color);
			vo.fills.push(fill);
		}
		this.flushBits();
	}
	,parseGradientFills: function(vo) {
		var cv = js.Lib.document.createElement("canvas");
		var g = cv.getContext("2d");
		var padding = this.readNBitCount();
		var gradientCount = this.readNBits(11);
		var _g = 0;
		while(_g < gradientCount) {
			var gc = _g++;
			var type = this.readNBits(3);
			var lineNBits = this.readNBitCount();
			var tlX = this.readNBitInt(lineNBits) / this.twips;
			var tlY = this.readNBitInt(lineNBits) / this.twips;
			var trX = this.readNBitInt(lineNBits) / this.twips;
			var trY = this.readNBitInt(lineNBits) / this.twips;
			var gradient;
			if(type == 0) gradient = g.createLinearGradient(tlX,tlY,trX,trY); else {
				var r2 = trX - tlX;
				gradient = g.createRadialGradient(tlX,tlY,0,tlX,tlY,r2);
			}
			var colorNBits = this.readNBitCount();
			var ratioNBits = this.readNBitCount();
			var count = this.readNBits(11);
			var _g1 = 0;
			while(_g1 < count) {
				var stops = _g1++;
				var color = ddw.Color.fromAFlipRGB(this.readNBits(colorNBits));
				var ratio = this.readNBits(ratioNBits) / 255;
				if(stops == 0 && ratio > 0) gradient.addColorStop(0,color.colorString);
				gradient.addColorStop(ratio,color.colorString);
			}
			var fill = new ddw.GradientFill(gradient);
			vo.fills.push(fill);
		}
		this.flushBits();
	}
	,parseSymbol: function(vo) {
		var result = new ddw.Symbol();
		result.id = this.readNBits(this.idBitCount);
		result.bounds = this.readNBitRect();
		var pathsCount = this.readNBits(11);
		var pathIndexNBits = this.readNBitCount();
		var _g = 0;
		while(_g < pathsCount) {
			var i = _g++;
			var path = new ddw.Path();
			var nBits = this.readNBitCount();
			var segmentCount = this.readNBits(11);
			var _g1 = 0;
			while(_g1 < segmentCount) {
				var j = _g1++;
				var seg = new ddw.Segment();
				var segType = this.readNBits(2);
				seg.points.push(this.readNBitInt(nBits) / this.twips);
				seg.points.push(this.readNBitInt(nBits) / this.twips);
				switch(segType) {
				case 0:
					seg.segmentType = 0;
					break;
				case 1:
					seg.segmentType = 1;
					break;
				case 2:
					seg.segmentType = 2;
					seg.points.push(this.readNBitInt(nBits) / this.twips);
					seg.points.push(this.readNBitInt(nBits) / this.twips);
					break;
				case 3:
					seg.segmentType = 3;
					seg.points.push(this.readNBitInt(nBits) / this.twips);
					seg.points.push(this.readNBitInt(nBits) / this.twips);
					seg.points.push(this.readNBitInt(nBits) / this.twips);
					seg.points.push(this.readNBitInt(nBits) / this.twips);
					break;
				}
				path.segments.push(seg);
			}
			result.paths.push(path);
		}
		var shapesCount = this.readNBits(11);
		var _g = 0;
		while(_g < shapesCount) {
			var i = _g++;
			var strokeIndex = this.readNBits(this.strokeIndexNBits);
			var fillIndex = this.readNBits(this.fillIndexNBits);
			var pathIndex = this.readNBits(pathIndexNBits);
			var shape = new ddw.Shape(strokeIndex,fillIndex,pathIndex);
			result.shapes.push(shape);
		}
		this.flushBits();
		return result;
	}
	,parseImage: function(vo) {
		var result = new ddw.Image();
		result.id = this.readNBits(this.idBitCount);
		result.bounds = this.readNBitRect();
		result.pathId = this.readNBits(11);
		result.setPath(vo.pathTable[result.pathId]);
		this.flushBits();
		return result;
	}
	,parseTimeline: function(vo) {
		var result = new ddw.Timeline();
		result.id = this.readNBits(this.idBitCount);
		result.bounds = this.readNBitRect();
		var instancesCount = this.readNBits(11);
		var _g = 0;
		while(_g < instancesCount) {
			var i = _g++;
			var defId = this.readNBits(this.idBitCount);
			var instId = this.readNBits(this.idBitCount);
			var inst = new ddw.Instance(defId,instId);
			inst.instanceId = instId;
			var hasX = this.readNBits(1) == 1?true:false;
			var hasY = this.readNBits(1) == 1?true:false;
			var hasScaleX = this.readNBits(1) == 1?true:false;
			var hasScaleY = this.readNBits(1) == 1?true:false;
			var hasRotation = this.readNBits(1) == 1?true:false;
			var hasShear = this.readNBits(1) == 1?true:false;
			var hasName = this.readNBits(1) == 1?true:false;
			if(hasX || hasY || hasScaleX || hasScaleY || hasRotation || hasShear) {
				var mxNBits = this.readNBitCount();
				if(hasX) inst.x = this.readNBitInt(mxNBits) / this.twips;
				if(hasY) inst.y = this.readNBitInt(mxNBits) / this.twips;
				if(hasScaleX) {
					inst.scaleX = this.readNBitInt(mxNBits) / this.twips;
					inst.hasScale = true;
				}
				if(hasScaleY) {
					inst.scaleY = this.readNBitInt(mxNBits) / this.twips;
					inst.hasScale = true;
				}
				if(hasRotation) {
					inst.rotation = this.readNBitInt(mxNBits) / this.twips;
					inst.hasRotation = true;
				}
				if(hasShear) {
					inst.shear = this.readNBitInt(mxNBits) / this.twips;
					inst.hasShear = true;
				}
			}
			if(hasName && vo.instanceNameLookupTable.exists(inst.instanceId)) inst.name = vo.instanceNameLookupTable.get(inst.instanceId);
			result.instances.push(inst);
		}
		this.flushBits();
		return result;
	}
	,parseStringTable: function(table) {
		var nameNBits = this.readNBitCount();
		var stringCount = this.readNBits(16);
		var _g = 0;
		while(_g < stringCount) {
			var i = _g++;
			var charCount = this.readNBits(16);
			var s = "";
			var _g1 = 0;
			while(_g1 < charCount) {
				var j = _g1++;
				var charVal = this.readNBits(nameNBits);
				s += String.fromCharCode(charVal);
			}
			table.push(s);
		}
		this.flushBits();
	}
	,parseNameTable: function(table,lookupTable) {
		var nameNBits = this.readNBitCount();
		var stringCount = this.readNBits(16);
		var _g = 0;
		while(_g < stringCount) {
			var i = _g++;
			var id = this.readNBitInt(this.idBitCount);
			var charCount = this.readNBits(16);
			var s = "";
			var _g1 = 0;
			while(_g1 < charCount) {
				var j = _g1++;
				var charVal = this.readNBits(nameNBits);
				s += String.fromCharCode(charVal);
			}
			if(table != null) table.set(s,id);
			if(lookupTable != null) lookupTable.set(id,s);
		}
		this.flushBits();
	}
	,mapNames: function(vo) {
		var $it0 = vo.definitionNameTable.keys();
		while( $it0.hasNext() ) {
			var s = $it0.next();
			var id = vo.definitionNameTable.get(s);
			var def = vo.definitions.get(id);
			if(js.Boot.__instanceof(def,ddw.Timeline)) {
				var tl = js.Boot.__cast(def , ddw.Timeline);
				tl.name = s;
			}
		}
	}
	,parseTags: function(vo) {
		this.index = 0;
		this.bit = 8;
		while(this.index < this.data.length) {
			var tag = this.data[this.index++];
			var len = this.readNBitInt(24);
			var startLoc = this.index;
			switch(tag) {
			case 32:
				this.parseNameTable(vo.definitionNameTable,null);
				break;
			case 33:
				this.parseNameTable(null,vo.instanceNameLookupTable);
				break;
			case 35:
				this.parseStringTable(vo.pathTable);
				break;
			case 5:
				this.parseStrokes(vo);
				break;
			case 6:
				this.parseSolidFills(vo);
				break;
			case 7:
				this.parseGradientFills(vo);
				break;
			case 16:
				var symbol = this.parseSymbol(vo);
				vo.definitions.set(symbol.id,symbol);
				break;
			case 17:
				var tl = this.parseTimeline(vo);
				vo.definitions.set(tl.id,tl);
				break;
			case 18:
				var img = this.parseImage(vo);
				vo.definitions.set(img.id,img);
				break;
			case 255:
				this.index += 0;
				break;
			default:
				this.index += len;
			}
			if(this.index - startLoc != len) {
				js.Lib.alert("Parse error. tagStart:" + startLoc + " tagEnd:" + this.index + " len:" + len + " tagType: " + tag);
				this.index = startLoc + len;
			}
		}
		this.mapNames(vo);
	}
	,__class__: ddw.VexDrawBinaryReader
}
ddw.VexDrawJsonReader = function(json,vo,onParseComplete) {
	this.parseJson(json,vo);
	if(onParseComplete != null) onParseComplete();
};
ddw.VexDrawJsonReader.__name__ = true;
ddw.VexDrawJsonReader.parseInstance = function(dinst) {
	var result = new ddw.Instance(dinst[0],dinst[1]);
	result.x = dinst[2][0];
	result.y = dinst[2][1];
	if(dinst.length > 3 && !js.Boot.__instanceof(dinst[3],String)) {
		var mxComp = dinst[3];
		result.scaleX = mxComp[0];
		result.scaleY = mxComp[1];
		result.hasScale = true;
		if(mxComp.length > 2) {
			result.rotation = mxComp[2];
			result.hasRotation = true;
		}
		if(mxComp.length > 3) {
			result.shear = mxComp[3];
			result.hasShear = true;
		}
	}
	if(dinst.length > 4) result.name = dinst[4]; else if(dinst.length > 3 && js.Boot.__instanceof(dinst[3],String)) result.name = dinst[3]; else result.name = "inst_" + result.instanceId;
	return result;
}
ddw.VexDrawJsonReader.parseSegment = function(seg) {
	var result = new ddw.Segment();
	var nums = seg.substring(1).split(",");
	switch(seg.charAt(0)) {
	case "M":
		result.segmentType = 0;
		result.points = [Std.parseFloat(nums[0]),Std.parseFloat(nums[1])];
		break;
	case "L":
		result.segmentType = 1;
		result.points = [Std.parseFloat(nums[0]),Std.parseFloat(nums[1])];
		break;
	case "Q":
		result.segmentType = 2;
		result.points = [Std.parseFloat(nums[0]),Std.parseFloat(nums[1]),Std.parseFloat(nums[2]),Std.parseFloat(nums[3])];
		break;
	case "C":
		result.segmentType = 3;
		result.points = [Std.parseFloat(nums[0]),Std.parseFloat(nums[1]),Std.parseFloat(nums[2]),Std.parseFloat(nums[3]),Std.parseFloat(nums[4]),Std.parseFloat(nums[5])];
		break;
	}
	return result;
}
ddw.VexDrawJsonReader.prototype = {
	parseFill: function(fill,g) {
		var result;
		if(js.Boot.__instanceof(fill,Array)) {
			var gradKind = fill[0];
			var tlbr = fill[1];
			var gradStops = fill[2];
			var gradient;
			if(gradKind == "L") gradient = g.createLinearGradient(tlbr[0],tlbr[1],tlbr[2],tlbr[3]); else {
				var difX = tlbr[2] - tlbr[0];
				var difY = tlbr[3] - tlbr[1];
				var r2 = Math.sqrt(difX * difX + difY * difY);
				gradient = g.createRadialGradient(tlbr[0],tlbr[1],0,tlbr[0],tlbr[1],r2);
			}
			var gs = 0;
			while(gs < gradStops.length) {
				var col = ddw.Color.fromAFlipRGB(gradStops[gs]);
				var colString = col.colorString;
				gradient.addColorStop(gradStops[gs + 1],colString);
				gs += 2;
			}
			result = new ddw.GradientFill(gradient);
		} else result = new ddw.SolidFill(ddw.Color.fromAFlipRGB(fill));
		return result;
	}
	,parseSymbol: function(dsym) {
		var symbol = new ddw.Symbol();
		symbol.id = dsym.id;
		symbol.bounds = new ddw.Rectangle(dsym.bounds[0],dsym.bounds[1],dsym.bounds[2],dsym.bounds[3]);
		var dPaths = dsym.paths;
		var _g = 0;
		while(_g < dPaths.length) {
			var dPath = dPaths[_g];
			++_g;
			var path = new ddw.Path();
			var segs = dPath.split(" ");
			var _g1 = 0;
			while(_g1 < segs.length) {
				var seg = segs[_g1];
				++_g1;
				var segment = ddw.VexDrawJsonReader.parseSegment(seg);
				path.segments.push(segment);
			}
			symbol.paths.push(path);
		}
		var dShapes = dsym.shapes;
		var _g = 0;
		while(_g < dShapes.length) {
			var dShape = dShapes[_g];
			++_g;
			var shape = new ddw.Shape(dShape[0],dShape[1],dShape[2]);
			symbol.shapes.push(shape);
		}
		return symbol;
	}
	,parseImage: function(dimg) {
		var img = new ddw.Image();
		img.id = dimg.id;
		img.bounds = new ddw.Rectangle(dimg.sourceRectangle[0],dimg.sourceRectangle[1],dimg.sourceRectangle[2],dimg.sourceRectangle[3]);
		img.setPath(dimg.path);
		return img;
	}
	,parseTimeline: function(dtl) {
		var result = new ddw.Timeline();
		result.isTimeline = true;
		result.id = dtl.id;
		result.name = dtl.name;
		result.bounds = new ddw.Rectangle(dtl.bounds[0],dtl.bounds[1],dtl.bounds[2],dtl.bounds[3]);
		var dInstances = dtl.instances;
		var _g = 0;
		while(_g < dInstances.length) {
			var dInst = dInstances[_g];
			++_g;
			var inst = ddw.VexDrawJsonReader.parseInstance(dInst);
			result.instances.push(inst);
		}
		return result;
	}
	,parseJson: function(data,vo,onParseComplete) {
		var dDefNames = data.definitionNameTable;
		var _g = 0;
		while(_g < dDefNames.length) {
			var def = dDefNames[_g];
			++_g;
			vo.definitionNameTable.set(def[1],def[0]);
		}
		var dInstNames = data.instanceNameTable;
		var _g = 0;
		while(_g < dInstNames.length) {
			var inst = dInstNames[_g];
			++_g;
			vo.instanceNameLookupTable.set(inst[0],inst[1]);
		}
		var i = 0;
		while(i < data.strokes.length) {
			var col = ddw.Color.fromAFlipRGB(data.strokes[i + 1]);
			var stroke = new ddw.Stroke(col,data.strokes[i]);
			vo.strokes.push(stroke);
			i += 2;
		}
		var cv = js.Lib.document.createElement("canvas");
		var g = cv.getContext("2d");
		var dFills = data.fills;
		var _g = 0;
		while(_g < dFills.length) {
			var dFill = dFills[_g];
			++_g;
			var f = this.parseFill(dFill,g);
			vo.fills.push(f);
			if(!f.isGradient) vo.gradientStart = i + 1;
		}
		var dImages = data.images;
		var _g = 0;
		while(_g < dImages.length) {
			var dImage = dImages[_g];
			++_g;
			var img = this.parseImage(dImage);
			vo.definitions.set(img.id,img);
		}
		var dSymbols = data.symbols;
		var _g = 0;
		while(_g < dSymbols.length) {
			var dSymbol = dSymbols[_g];
			++_g;
			var symbol = this.parseSymbol(dSymbol);
			vo.definitions.set(symbol.id,symbol);
		}
		var dTimelines = data.timelines;
		var _g = 0;
		while(_g < dTimelines.length) {
			var dtl = dTimelines[_g];
			++_g;
			var tl = this.parseTimeline(dtl);
			vo.definitions.set(tl.id,tl);
			if(tl.name != null) vo.definitionNameTable.set(tl.name,tl.id);
		}
		if(onParseComplete != null) onParseComplete();
	}
	,__class__: ddw.VexDrawJsonReader
}
ddw.VexDrawTag = function() { }
ddw.VexDrawTag.__name__ = true;
ddw.VexObject = function() {
	this.boxSize = 25;
	this.gradientStart = 0;
	this.fills = new Array();
	this.strokes = new Array();
	this.definitionNameTable = new Hash();
	this.instanceNameLookupTable = new IntHash();
	this.pathTable = new Array();
	this.definitions = new IntHash();
	this.timelineStack = new Array();
	this.timelineStack.push(document.body);
};
ddw.VexObject.__name__ = true;
ddw.VexObject.prototype = {
	drawColorTables: function() {
		var inst = new ddw.Instance(0);
		inst.y = 40;
		var i = 0;
		var _g = 0, _g1 = this.strokes;
		while(_g < _g1.length) {
			var stroke = _g1[_g];
			++_g;
			var cv = this.createCanvas("st_" + stroke.lineWidth + "_" + stroke.color.getColorHex(),this.boxSize,this.boxSize);
			inst.x = (this.boxSize + 2) * i++;
			this.transformObject(cv,inst,0,0);
			var g = cv.getContext("2d");
			g.fillStyle = this.fills[0];
			g.lineWidth = stroke.lineWidth;
			g.strokeStyle = stroke.color.colorString;
			g.strokeRect(0,0,this.boxSize,this.boxSize);
		}
		i = 0;
		var solidCount = 0;
		var gradCount = 0;
		var _g = 0, _g1 = this.fills;
		while(_g < _g1.length) {
			var fill = _g1[_g];
			++_g;
			var id = fill.isGradient?"grad_" + gradCount:"sf_" + (js.Boot.__cast(fill , ddw.SolidFill)).color.getColorHex();
			inst.x = fill.isGradient?(this.boxSize + 2) * gradCount++:(this.boxSize + 2) * solidCount++;
			inst.y = fill.isGradient?100:70;
			var cv = this.createCanvas(id,this.boxSize,this.boxSize);
			this.transformObject(cv,inst,0,0);
			var g = cv.getContext("2d");
			g.fillStyle = fill.canvasFill;
			g.fillRect(0,0,this.boxSize,this.boxSize);
		}
	}
	,drawTimeline: function(index) {
		var tlDef = this.definitions.get(index);
		if(tlDef != null && js.Boot.__instanceof(tlDef,ddw.Timeline)) {
			var tl = tlDef;
			ddw.Timeline.drawTimeline(tl,this);
		}
	}
	,drawTimelineByName: function(name) {
		if(this.definitionNameTable.exists(name)) {
			var id = this.definitionNameTable.get(name);
			this.drawTimeline(id);
		}
	}
	,transformObject: function(obj,instance,offsetX,offsetY) {
		if(instance.x != 0 || instance.y != 0 || offsetX != 0 || offsetY != 0) {
			var orgTxt = offsetX + "px " + offsetY + "px";
			obj.style.WebkitTransformOrigin = orgTxt;
			obj.style.msTransformOrigin = orgTxt;
			obj.style.OTransformOrigin = orgTxt;
			obj.style.MozTransformOrigin = orgTxt;
			var trans = "";
			var orgX = instance.x - offsetX;
			var orgY = instance.y - offsetY;
			trans += "translate(" + orgX + "px," + orgY + "px)";
			if(instance.hasShear) trans += "skewX(" + instance.shear + ") ";
			if(instance.hasRotation) trans += "rotate(" + instance.rotation + "deg) ";
			obj.style.WebkitTransform = trans;
			obj.style.msTransform = trans;
			obj.style.OTransform = trans;
			obj.style.MozTransform = trans;
		}
	}
	,createCanvas: function(id,width,height) {
		var canvas = document.createElement("canvas");
		canvas.id = id;
		canvas.width = width;
		canvas.height = height;
		this.timelineStack[0].appendChild(canvas);
		return canvas;
	}
	,popDiv: function() {
		this.timelineStack.shift();
	}
	,pushDiv: function(id) {
		var div = js.Lib.document.createElement("div");
		div.id = id;
		this.timelineStack[0].appendChild(div);
		this.timelineStack.unshift(div);
		return div;
	}
	,parseBinaryFile: function(path,onParseComplete) {
		var vdbr = new ddw.VexDrawBinaryReader(path,this,onParseComplete);
	}
	,parseJson: function(json,onParseComplete) {
		var vdbr = new ddw.VexDrawJsonReader(json,this,onParseComplete);
	}
	,__class__: ddw.VexObject
}
var haxe = haxe || {}
haxe.StackItem = { __ename__ : true, __constructs__ : ["CFunction","Module","FilePos","Method","Lambda"] }
haxe.StackItem.CFunction = ["CFunction",0];
haxe.StackItem.CFunction.toString = $estr;
haxe.StackItem.CFunction.__enum__ = haxe.StackItem;
haxe.StackItem.Module = function(m) { var $x = ["Module",1,m]; $x.__enum__ = haxe.StackItem; $x.toString = $estr; return $x; }
haxe.StackItem.FilePos = function(s,file,line) { var $x = ["FilePos",2,s,file,line]; $x.__enum__ = haxe.StackItem; $x.toString = $estr; return $x; }
haxe.StackItem.Method = function(classname,method) { var $x = ["Method",3,classname,method]; $x.__enum__ = haxe.StackItem; $x.toString = $estr; return $x; }
haxe.StackItem.Lambda = function(v) { var $x = ["Lambda",4,v]; $x.__enum__ = haxe.StackItem; $x.toString = $estr; return $x; }
haxe.Stack = function() { }
haxe.Stack.__name__ = true;
haxe.Stack.callStack = function() {
	var oldValue = Error.prepareStackTrace;
	Error.prepareStackTrace = function(error,callsites) {
		var stack = [];
		var _g = 0;
		while(_g < callsites.length) {
			var site = callsites[_g];
			++_g;
			var method = null;
			var fullName = site.getFunctionName();
			if(fullName != null) {
				var idx = fullName.lastIndexOf(".");
				if(idx >= 0) {
					var className = HxOverrides.substr(fullName,0,idx);
					var methodName = HxOverrides.substr(fullName,idx + 1,null);
					method = haxe.StackItem.Method(className,methodName);
				}
			}
			stack.push(haxe.StackItem.FilePos(method,site.getFileName(),site.getLineNumber()));
		}
		return stack;
	};
	var a = haxe.Stack.makeStack(new Error().stack);
	a.shift();
	Error.prepareStackTrace = oldValue;
	return a;
}
haxe.Stack.exceptionStack = function() {
	return [];
}
haxe.Stack.toString = function(stack) {
	var b = new StringBuf();
	var _g = 0;
	while(_g < stack.length) {
		var s = stack[_g];
		++_g;
		b.b += Std.string("\nCalled from ");
		haxe.Stack.itemToString(b,s);
	}
	return b.b;
}
haxe.Stack.itemToString = function(b,s) {
	var $e = (s);
	switch( $e[1] ) {
	case 0:
		b.b += Std.string("a C function");
		break;
	case 1:
		var m = $e[2];
		b.b += Std.string("module ");
		b.b += Std.string(m);
		break;
	case 2:
		var line = $e[4], file = $e[3], s1 = $e[2];
		if(s1 != null) {
			haxe.Stack.itemToString(b,s1);
			b.b += Std.string(" (");
		}
		b.b += Std.string(file);
		b.b += Std.string(" line ");
		b.b += Std.string(line);
		if(s1 != null) b.b += Std.string(")");
		break;
	case 3:
		var meth = $e[3], cname = $e[2];
		b.b += Std.string(cname);
		b.b += Std.string(".");
		b.b += Std.string(meth);
		break;
	case 4:
		var n = $e[2];
		b.b += Std.string("local function #");
		b.b += Std.string(n);
		break;
	}
}
haxe.Stack.makeStack = function(s) {
	if(typeof(s) == "string") {
		var stack = s.split("\n");
		var m = [];
		var _g = 0;
		while(_g < stack.length) {
			var line = stack[_g];
			++_g;
			m.push(haxe.StackItem.Module(line));
		}
		return m;
	} else return s;
}
var js = js || {}
js.Boot = function() { }
js.Boot.__name__ = true;
js.Boot.__unhtml = function(s) {
	return s.split("&").join("&amp;").split("<").join("&lt;").split(">").join("&gt;");
}
js.Boot.__trace = function(v,i) {
	var msg = i != null?i.fileName + ":" + i.lineNumber + ": ":"";
	msg += js.Boot.__string_rec(v,"");
	var d;
	if(typeof(document) != "undefined" && (d = document.getElementById("haxe:trace")) != null) d.innerHTML += js.Boot.__unhtml(msg) + "<br/>"; else if(typeof(console) != "undefined" && console.log != null) console.log(msg);
}
js.Boot.__clear_trace = function() {
	var d = document.getElementById("haxe:trace");
	if(d != null) d.innerHTML = "";
}
js.Boot.isClass = function(o) {
	return o.__name__;
}
js.Boot.isEnum = function(e) {
	return e.__ename__;
}
js.Boot.getClass = function(o) {
	return o.__class__;
}
js.Boot.__string_rec = function(o,s) {
	if(o == null) return "null";
	if(s.length >= 5) return "<...>";
	var t = typeof(o);
	if(t == "function" && (o.__name__ || o.__ename__)) t = "object";
	switch(t) {
	case "object":
		if(o instanceof Array) {
			if(o.__enum__) {
				if(o.length == 2) return o[0];
				var str = o[0] + "(";
				s += "\t";
				var _g1 = 2, _g = o.length;
				while(_g1 < _g) {
					var i = _g1++;
					if(i != 2) str += "," + js.Boot.__string_rec(o[i],s); else str += js.Boot.__string_rec(o[i],s);
				}
				return str + ")";
			}
			var l = o.length;
			var i;
			var str = "[";
			s += "\t";
			var _g = 0;
			while(_g < l) {
				var i1 = _g++;
				str += (i1 > 0?",":"") + js.Boot.__string_rec(o[i1],s);
			}
			str += "]";
			return str;
		}
		var tostr;
		try {
			tostr = o.toString;
		} catch( e ) {
			return "???";
		}
		if(tostr != null && tostr != Object.toString) {
			var s2 = o.toString();
			if(s2 != "[object Object]") return s2;
		}
		var k = null;
		var str = "{\n";
		s += "\t";
		var hasp = o.hasOwnProperty != null;
		for( var k in o ) { ;
		if(hasp && !o.hasOwnProperty(k)) {
			continue;
		}
		if(k == "prototype" || k == "__class__" || k == "__super__" || k == "__interfaces__" || k == "__properties__") {
			continue;
		}
		if(str.length != 2) str += ", \n";
		str += s + k + " : " + js.Boot.__string_rec(o[k],s);
		}
		s = s.substring(1);
		str += "\n" + s + "}";
		return str;
	case "function":
		return "<function>";
	case "string":
		return o;
	default:
		return String(o);
	}
}
js.Boot.__interfLoop = function(cc,cl) {
	if(cc == null) return false;
	if(cc == cl) return true;
	var intf = cc.__interfaces__;
	if(intf != null) {
		var _g1 = 0, _g = intf.length;
		while(_g1 < _g) {
			var i = _g1++;
			var i1 = intf[i];
			if(i1 == cl || js.Boot.__interfLoop(i1,cl)) return true;
		}
	}
	return js.Boot.__interfLoop(cc.__super__,cl);
}
js.Boot.__instanceof = function(o,cl) {
	try {
		if(o instanceof cl) {
			if(cl == Array) return o.__enum__ == null;
			return true;
		}
		if(js.Boot.__interfLoop(o.__class__,cl)) return true;
	} catch( e ) {
		if(cl == null) return false;
	}
	switch(cl) {
	case Int:
		return Math.ceil(o%2147483648.0) === o;
	case Float:
		return typeof(o) == "number";
	case Bool:
		return o === true || o === false;
	case String:
		return typeof(o) == "string";
	case Dynamic:
		return true;
	default:
		if(o == null) return false;
		if(cl == Class && o.__name__ != null) return true; else null;
		if(cl == Enum && o.__ename__ != null) return true; else null;
		return o.__enum__ == cl;
	}
}
js.Boot.__cast = function(o,t) {
	if(js.Boot.__instanceof(o,t)) return o; else throw "Cannot cast " + Std.string(o) + " to " + Std.string(t);
}
js.Lib = function() { }
js.Lib.__name__ = true;
js.Lib.debug = function() {
	debugger;
}
js.Lib.alert = function(v) {
	alert(js.Boot.__string_rec(v,""));
}
js.Lib.eval = function(code) {
	return eval(code);
}
js.Lib.setErrorHandler = function(f) {
	js.Lib.onerror = f;
}
if(Array.prototype.indexOf) HxOverrides.remove = function(a,o) {
	var i = a.indexOf(o);
	if(i == -1) return false;
	a.splice(i,1);
	return true;
}; else null;
Math.__name__ = ["Math"];
Math.NaN = Number.NaN;
Math.NEGATIVE_INFINITY = Number.NEGATIVE_INFINITY;
Math.POSITIVE_INFINITY = Number.POSITIVE_INFINITY;
Math.isFinite = function(i) {
	return isFinite(i);
};
Math.isNaN = function(i) {
	return isNaN(i);
};
String.prototype.__class__ = String;
String.__name__ = true;
Array.prototype.__class__ = Array;
Array.__name__ = true;
Date.prototype.__class__ = Date;
Date.__name__ = ["Date"];
var Int = { __name__ : ["Int"]};
var Dynamic = { __name__ : ["Dynamic"]};
var Float = Number;
Float.__name__ = ["Float"];
var Bool = Boolean;
Bool.__ename__ = ["Bool"];
var Class = { __name__ : ["Class"]};
var Enum = { };
var Void = { __ename__ : ["Void"]};
if(typeof document != "undefined") js.Lib.document = document;
if(typeof window != "undefined") {
	js.Lib.window = window;
	js.Lib.window.onerror = function(msg,url,line) {
		var f = js.Lib.onerror;
		if(f == null) return false;
		return f(msg,[url + ":" + line]);
	};
}
ddw.Instance.instanceCounter = 0;
ddw.SegmentType.MoveTo = 0;
ddw.SegmentType.LineTo = 1;
ddw.SegmentType.QuadraticCurveTo = 2;
ddw.SegmentType.BezierCurveTo = 3;
ddw.VexDrawTag.None = 0;
ddw.VexDrawTag.Header = 1;
ddw.VexDrawTag.StrokeList = 5;
ddw.VexDrawTag.SolidFillList = 6;
ddw.VexDrawTag.GradientFillList = 7;
ddw.VexDrawTag.ReplacementSolidFillList = 9;
ddw.VexDrawTag.ReplacementGradientFillList = 10;
ddw.VexDrawTag.ReplacementStrokeList = 11;
ddw.VexDrawTag.SymbolDefinition = 16;
ddw.VexDrawTag.TimelineDefinition = 17;
ddw.VexDrawTag.ImageDefinition = 18;
ddw.VexDrawTag.DefinitionNameTable = 32;
ddw.VexDrawTag.InstanceNameTable = 33;
ddw.VexDrawTag.ColorNameTable = 34;
ddw.VexDrawTag.PathNameTable = 35;
ddw.VexDrawTag.End = 255;
Main.main();

//@ sourceMappingURL=vexDrawHaxe.js.map