//
//  main.m
//  HashTable
//
//  Created by Aleksey Bodnya on 5/16/14.
//  Copyright (c) 2014 Alexey Bodnya. All rights reserved.
//

#include <CoreFoundation/CoreFoundation.h>
#include "TRHashTableTests.h"

int main(int argc, const char * argv[])
{
    TRHashTableTests *tests = [TRHashTableTests new];
    [tests testWithObjectsCount:10000];
    
    return 0;
}


