//
//  TRHashTableTests.m
//  HashTable
//
//  Created by Aleksey Bodnya on 5/16/14.
//  Copyright (c) 2014 Alexey Bodnya. All rights reserved.
//

#import "TRHashTableTests.h"
#import "TRHashTableChainMethod.h"
#import "TRHashTableSimple.h"
#import "NSString+RandomString.h"

static const NSUInteger maxStringLength = 10;

@interface TRHashTableTests ()

@property (nonatomic, strong) NSMutableArray *testStringValues;

@end

@implementation TRHashTableTests

- (void)testWithObjectsCount:(NSUInteger)count
{
    self.testStringValues = [NSMutableArray array];
    for (NSUInteger index = 0; index < count; index++)
    {
        [self.testStringValues addObject:[NSString randomStringWithLength:maxStringLength]];
    }
    [self testChainMethod];
    [self testSimple];
}

- (void)testSimple
{
    NSDate *startDate = NSDate.date;
    TRHashTableSimple *table = [TRHashTableSimple tableWithSize:200000];
    for (NSString *value in self.testStringValues)
    {
        [table addStringValue:value];
    }
    NSLog(@"Time Simple: %lf",[NSDate.date timeIntervalSinceDate:startDate]);
//    NSLog(@"%@",table);
}

- (void)testChainMethod
{
    NSDate *startDate = NSDate.date;
    TRHashTableChainMethod *table = [TRHashTableChainMethod tableWithSize:200000];
    for (NSString *value in self.testStringValues)
    {
        [table addStringValue:value];
    }
    NSLog(@"Time Chains: %lf",[NSDate.date timeIntervalSinceDate:startDate]);
//    NSLog(@"%@",table);
}

@end
