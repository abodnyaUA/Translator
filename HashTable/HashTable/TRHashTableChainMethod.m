//
//  TRHashTableChainMethod.m
//  HashTable
//
//  Created by Aleksey Bodnya on 5/16/14.
//  Copyright (c) 2014 Alexey Bodnya. All rights reserved.
//

#import "TRHashTableChainMethod.h"

@interface TRChain : NSObject

@property (nonatomic, assign) NSUInteger key;
@property (nonatomic, strong) NSString *value;
@property (nonatomic, strong) TRChain *next;

+ (instancetype)chainWithKey:(NSUInteger)key value:(NSString *)value;
- (void)addChainWithValue:(NSString *)value;
- (NSArray *)allValues;

@end

@implementation TRChain

+ (instancetype)chainWithKey:(NSUInteger)key value:(NSString *)value
{
    TRChain *chain = [TRChain new];
    chain.key = key;
    chain.value = value;
    return chain;
}

- (void)addChainWithValue:(NSString *)value
{
    TRChain *last = self;
    
    while (nil != last.next)
    {
        last = last.next;
    }
    
    last.next = [TRChain chainWithKey:self.key value:value];
}

- (NSArray *)allValues
{
    NSMutableArray *values = [NSMutableArray array];
    if (nil != self.value)
    {
        [values addObject:self.value];
        TRChain *last = self;
        
        while (nil != last.next)
        {
            last = last.next;
            [values addObject:last.value];
        }
    }
    return [values copy];
}

@end


@interface TRHashTableChainMethod ()

@property (nonatomic, strong) NSMutableArray *chains;
@property (nonatomic, assign) NSUInteger size;
@property (nonatomic, assign) NSUInteger entriesCount;

@end

@implementation TRHashTableChainMethod

+ (instancetype)tableWithSize:(NSUInteger)size
{
    TRHashTableChainMethod *table = [TRHashTableChainMethod new];
    table.chains = [NSMutableArray array];
    table.size = size;
    table.entriesCount = 0;
    return table;
}

- (NSUInteger)hashIndex
{
    return self.entriesCount % self.size;
}

- (NSUInteger)indexOfValueWithKey:(NSUInteger)key
{
    if (self.chains.count > 0)
    {
        NSInteger left = 0;
        NSInteger right = self.chains.count - 1;
        NSInteger mid = 0;
        while (left <= right)
        {
            mid = left + (right - left) / 2;
            if (key < [(TRChain *)self.chains[mid] key])
            {
                right = mid - 1;
            }
            else if (key > [(TRChain *)self.chains[mid] key])
            {
                left = mid + 1;
            }
            else
            {
                return mid;
            }
        }
    }
    return NSNotFound;
}

- (TRChain *)chainWithKey:(NSUInteger)key
{
    NSUInteger index = [self indexOfValueWithKey:key];
    return NSNotFound != index ? [self.chains objectAtIndex:index] : nil;
}

- (NSUInteger)addStringValue:(NSString *)value
{
    NSUInteger key = self.hashIndex;
    
    NSInteger left = 0, right = self.chains.count - 1, mid = 0, midKey;
    BOOL found = NO;
    if (self.chains.count > 0)
    {
        while (left <= right && !found)
        {
            mid = left + (right - left) / 2;
            midKey = [(TRChain *)self.chains[mid] key];
            if (key < midKey)
            {
                right = mid - 1;
            }
            else if (key > midKey)
            {
                left = mid + 1;
            }
            else
            {
                found = YES;
            }
        }
    }
    
    TRChain *chain = self.chains.count && found > 0 ? [self.chains objectAtIndex:mid] : nil;
    if (nil != chain.value)
    {
        [chain addChainWithValue:value];
    }
    else
    {
        [self.chains insertObject:[TRChain chainWithKey:key value:value] atIndex:left];
    }
    self.entriesCount++;
    return key;
}

- (void)removeStringValue:(NSString *)value forKey:(NSUInteger)key
{
    TRChain *chain = [self chainWithKey:key];
    if (nil != chain)
    {
        [self.chains removeObject:chain];
    }
}

- (NSArray *)valuesForKey:(NSUInteger)key
{
    NSArray *values = nil;
    TRChain *chain = [self chainWithKey:key];
    if (nil != chain)
    {
        values = [chain allValues];
    }
    return values;
}

- (NSString *)description
{
    NSMutableString *description = [NSMutableString stringWithString:[super description]];
    [description appendString:@"\n"];
    NSArray *sorted = [self.chains sortedArrayUsingDescriptors:@[[NSSortDescriptor sortDescriptorWithKey:@"key" ascending:YES]]];
    for (TRChain *chain in sorted)
    {
        [description appendFormat:@"key: %ld; values: %@",(long)chain.key,chain.allValues];
    }
    return description;
}

@end
