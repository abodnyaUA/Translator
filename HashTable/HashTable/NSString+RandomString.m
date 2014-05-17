//
//  NSString+RandomString.m
//  HashTable
//
//  Created by Aleksey Bodnya on 5/16/14.
//  Copyright (c) 2014 Alexey Bodnya. All rights reserved.
//

#import "NSString+RandomString.h"

static const NSString *letters = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

@implementation NSString (RandomString)

+ (NSString *)randomStringWithLength:(int)length
{
    NSMutableString *randomString = [NSMutableString stringWithCapacity:length];
    for (int i=0; i<length; i++)
    {
        [randomString appendFormat:@"%C", [letters characterAtIndex:(arc4random() % letters.length)]];
    }
    
    return [randomString copy];
}

@end
