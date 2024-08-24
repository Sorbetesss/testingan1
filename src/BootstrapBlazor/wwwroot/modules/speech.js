﻿import Data from "./data.js"

export async function speak(id, invoke, option) {
    const synth = window.speechSynthesis;
    if (synth.speaking) {
        invoke.invokeMethodAsync("OnSpeaking");
        return;
    }

    const { text, lang, pitch, rate, voice, volume } = option;
    if (text !== "") {
        const utter = new SpeechSynthesisUtterance(text);
        if (lang) {
            utter.lang = lang;
        }
        if (pitch) {
            utter.pitch = pitch;
        }
        if (rate) {
            utter.rate = rate;
        }
        if (voice) {
            const voices = await getUtteranceVoices();
            utter.voice = voices.find(v => v.name === voice.name);
        }
        if (volume) {
            utter.volume = volume;
        }

        utter.onend = () => {
            invoke.invokeMethodAsync("OnEnd");
        };

        utter.onerror = e => {
            invoke.invokeMethodAsync("OnError");
        };
        synth.speak(utter);
    }
}

export function pause(id) {

}

export function resume(id) {

}

export function cancel(id) {

}

const getUtteranceVoices = () => {
    const synth = window.speechSynthesis;
    let done = false;
    let voices = [];
    if (synth.onvoiceschanged === null) {
        synth.onvoiceschanged = () => {
            voices = synth.getVoices();
            done = true;
        };
    }
    else {
        voices = synth.getVoices();
        done = true;
    }

    return new Promise((resolve, reject) => {
        const handler = setInterval(() => {
            if (done) {
                clearInterval(handler);
                resolve(voices);
            }
        }, 10)
    })
}

export async function getVoices() {
    const voices = await getUtteranceVoices();
    return voices.map(i => {
        return {
            default: i.default,
            lang: i.lang,
            localService: i.localService,
            name: i.name,
            voiceURI: i.voiceURI
        }
    });
}