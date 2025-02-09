#ifndef OPERATOR_UTILITY
#define OPERATOR_UTILITY

uint resolveIndex(uint threadId, int inputlength)
{
    return min(threadId, inputlength - 1);
}

#endif
