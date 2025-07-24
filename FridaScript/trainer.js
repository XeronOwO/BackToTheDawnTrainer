"use strict";
(() => {
    class DiceCheat {
        constructor(module, methodRVAs) {
            this.diceValues = [null, null];
            this.isRandomTwoEntered = false;
            this.randomOneEnterCount = 0;
            this.attachInterceptor(module, methodRVAs);
            this.recvDiceCheatValues();
        }
        attachInterceptor(module, methodRVAs) {
            const randomTwoDiceValueMethodSignature = 'void ThrowDiceRangePoint___RandomTwoDiceValue (int32_t* dice1Value, int32_t* dice2value, bool isChallengeDice, const MethodInfo* method);';
            const randomOneIntValueMethodSignature = 'int32_t CommonManage__RandomOneIntValue (int32_t minValue, int32_t maxValue, const MethodInfo* method);';
            let randomTwoDiceValueMethodRVA = methodRVAs[randomTwoDiceValueMethodSignature];
            if (randomTwoDiceValueMethodRVA === undefined) {
                throw new Error('Method RVA for RandomTwoDiceValue not found.');
            }
            let randomOneIntValueMethodRVA = methodRVAs[randomOneIntValueMethodSignature];
            if (randomOneIntValueMethodRVA === undefined) {
                throw new Error('Method RVA for RandomOneIntValue not found.');
            }
            Interceptor.attach(module.base.add(randomTwoDiceValueMethodRVA), {
                onEnter: args => this.onEnterRandomTwoDice(args),
                onLeave: retval => this.onLeaveRandomTwoDice(retval),
            });
            Interceptor.attach(module.base.add(randomOneIntValueMethodRVA), {
                onLeave: retval => this.onLeaveRandomOneIntValue(retval),
            });
            console.log('Dice cheat initialized');
        }
        onEnterRandomTwoDice(args) {
            this.isRandomTwoEntered = true;
            this.randomOneEnterCount = 0;
            console.log('Enter RandomTwoDiceValue');
        }
        onLeaveRandomTwoDice(retval) {
            this.isRandomTwoEntered = false;
            console.log('Leave RandomTwoDiceValue');
        }
        onLeaveRandomOneIntValue(retval) {
            if (!this.isRandomTwoEntered) {
                return;
            }
            ++this.randomOneEnterCount;
            if (this.randomOneEnterCount > this.diceValues.length) {
                return;
            }
            let index = this.randomOneEnterCount - 1;
            let diceValue = this.diceValues[index];
            if (diceValue !== null) {
                let oldValue = retval.toInt32();
                retval.replace(ptr(diceValue));
                console.log(`Changed dice ${this.randomOneEnterCount} value from ${oldValue} to ${diceValue}`);
            }
            console.log('Leave RandomOneIntValue');
        }
        recvDiceCheatValues() {
            recv('DiceCheatValues', message => this.onDiceCheatValuesReceived(message));
        }
        onDiceCheatValuesReceived(message) {
            this.diceValues = message.payload;
            console.log(`Received dice values: ${JSON.stringify(this.diceValues)}`);
            this.recvDiceCheatValues();
        }
    }
    class GambleCheat {
        constructor(module, methodRVAs) {
            this.incomingGambleValues = [null, null];
            this.outgoingGambleValues = [];
            this.isSendGamblerCardEntered = false;
            this.randomOneIntFromListEnterCount = 0;
            this.attachInterceptor(module, methodRVAs);
            this.recvGambleCheatValues();
        }
        attachInterceptor(module, methodRVAs) {
            const sendGamblerCardMethodSignature = 'void GambleRound__SendGamblerCard (System_Collections_Generic_List_Gambler__o* specifyGamblerList, const MethodInfo* method);';
            const randomOneIntFromListMethodSignature = 'int32_t CommonManage__RandomOneIntFromList (System_Collections_Generic_List_int__o* list, const MethodInfo* method);';
            let sendGamblerCardMethodRVA = methodRVAs[sendGamblerCardMethodSignature];
            if (sendGamblerCardMethodRVA === undefined) {
                throw new Error('Method RVA for SendGamblerCard not found.');
            }
            let randomOneIntFromListMethodRVA = methodRVAs[randomOneIntFromListMethodSignature];
            if (randomOneIntFromListMethodRVA === undefined) {
                throw new Error('Method RVA for RandomOneIntFromList not found.');
            }
            Interceptor.attach(module.base.add(sendGamblerCardMethodRVA), {
                onEnter: args => this.onEnterSendGamblerCard(args),
                onLeave: retval => this.onLeaveSendGamblerCard(retval),
            });
            Interceptor.attach(module.base.add(randomOneIntFromListMethodRVA), {
                onLeave: retval => this.onLeaveRandomOneIntFromList(retval),
            });
            console.log('Gamble cheat initialized');
        }
        onEnterSendGamblerCard(args) {
            this.isSendGamblerCardEntered = true;
            this.randomOneIntFromListEnterCount = 0;
            this.outgoingGambleValues = [];
            console.log('Enter SendGamblerCard');
        }
        onLeaveSendGamblerCard(retval) {
            this.isSendGamblerCardEntered = false;
            send({ 'type': 'GambleCardValues', 'data': this.outgoingGambleValues });
            console.log(`Cards sent: ${JSON.stringify(this.outgoingGambleValues)}`);
            console.log('Leave SendGamblerCard');
        }
        onLeaveRandomOneIntFromList(retval) {
            if (!this.isSendGamblerCardEntered) {
                return;
            }
            ++this.randomOneIntFromListEnterCount;
            if (this.randomOneIntFromListEnterCount > this.incomingGambleValues.length) {
                return;
            }
            let index = this.randomOneIntFromListEnterCount - 1;
            let gambleValue = this.incomingGambleValues[index];
            if (gambleValue !== null) {
                let oldValue = retval.toInt32();
                retval.replace(ptr(gambleValue));
                this.outgoingGambleValues.push(gambleValue);
                console.log(`Changed gamble ${this.randomOneIntFromListEnterCount} value from ${oldValue} to ${gambleValue}`);
            }
            else {
                this.outgoingGambleValues.push(retval.toInt32());
            }
            console.log('Leave RandomOneIntFromList');
        }
        recvGambleCheatValues() {
            recv('GambleCheatValues', message => this.onGambleCheatValuesReceived(message));
        }
        onGambleCheatValuesReceived(message) {
            this.incomingGambleValues = message.payload;
            console.log(`Received gamble values: ${JSON.stringify(this.incomingGambleValues)}`);
            this.recvGambleCheatValues();
        }
    }
    class Trainer {
        constructor(module) {
            this.methodRVAs = {};
            this.module = module;
            recv('MethodRVAs', message => this.onMethodRVAsReceived(message));
            send({ 'type': 'Init' });
        }
        onMethodRVAsReceived(message) {
            this.methodRVAs = message.payload;
            this.initDiceCheat();
            this.initGambleCheat();
            send({ 'type': 'Ready' });
        }
        initDiceCheat() {
            try {
                new DiceCheat(this.module, this.methodRVAs);
            }
            catch (error) {
                console.error('Dice cheating will not work because errors occurred: ', error.message);
            }
        }
        initGambleCheat() {
            try {
                new GambleCheat(this.module, this.methodRVAs);
            }
            catch (error) {
                console.error('Gamble cheating will not work because errors occurred: ', error.message);
            }
        }
    }
    let targetModule = Process.findModuleByName('GameAssembly.dll');
    if (targetModule !== null) {
        new Trainer(targetModule);
    }
    else {
        Process.attachModuleObserver({
            onAdded(module) {
                if (module.name === 'GameAssembly.dll') {
                    new Trainer(module);
                }
            },
        });
    }
    recv('Exit', message => {
        Interceptor.detachAll();
    });
})();
