﻿Shader "Unlit Color Only" {
 
Properties {
    _Color ("Color", Color) = (1,1,1)
}
 
SubShader {
    Color [_Color]
    ZTest Always
    ZWrite Off
    Pass {}
}
 
}