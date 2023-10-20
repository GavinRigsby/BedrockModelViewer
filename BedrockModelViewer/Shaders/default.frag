#version 400 core

in vec2 texCoord;

out vec4 FragColor;

uniform sampler2D texture0;

void main()
{
    float alpha = texture(texture0, texCoord).a;

    
    if (alpha < 1){
        discard;
    }

    FragColor = texture(texture0, texCoord);
}
