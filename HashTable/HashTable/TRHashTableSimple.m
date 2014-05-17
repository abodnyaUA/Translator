//
//  TRHashTableSimple.m
//  HashTable
//
//  Created by Aleksey Bodnya on 5/17/14.
//  Copyright (c) 2014 Alexey Bodnya. All rights reserved.
//

#import "TRHashTableSimple.h"

@interface TRHashEntry : NSObject

@property (nonatomic, assign) NSUInteger key;
@property (nonatomic, strong) NSString *value;

+ (instancetype)entryWithKey:(NSUInteger)key value:(NSString *)value;

@end

@implementation TRHashEntry

+ (instancetype)entryWithKey:(NSUInteger)key value:(NSString *)value
{
    TRHashEntry *entry = [TRHashEntry new];
    entry.key = key;
    entry.value = value;
    return entry;
}

@end

@interface TRHashTableSimple ()

@property (nonatomic, strong) NSMutableOrderedSet *values;
@property (nonatomic, assign) NSUInteger size;
@property (nonatomic, assign) NSUInteger entriesCount;

@end

@implementation TRHashTableSimple

+ (instancetype)tableWithSize:(NSUInteger)size
{
    TRHashTableSimple *table = [TRHashTableSimple new];
    table.values = [NSMutableOrderedSet orderedSet];
    table.size = size;
    table.entriesCount = 0;
    return table;
}

- (NSUInteger)indexOfValueWithKey:(NSUInteger)key
{
    if (self.values.count > 0)
    {
        NSUInteger left = 0, right = self.values.count - 1, mid = 0;
        while (left <= right)
        {
            mid = left + (right - left) / 2;
            if (key < [(TRHashEntry *)self.values[mid] key])
            {
                right = mid - 1;
            }
            else if (key > [(TRHashEntry *)self.values[mid] key])
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

- (NSUInteger)indexForAddValueWithKey:(NSUInteger)key
{
    NSUInteger left = 0, right = self.values.count - 1, mid = 0;
    if (self.values.count > 0)
    {
        while (left <= right)
        {
            mid = left + (right - left) / 2;
            if (key < [(TRHashEntry *)self.values[mid] key])
            {
                right = mid - 1;
            }
            else if (key > [(TRHashEntry *)self.values[mid] key])
            {
                left = mid + 1;
            }
        }
    }
    return left;
}

- (TRHashEntry *)entryWithKey:(NSUInteger)key
{
    NSUInteger index = [self indexOfValueWithKey:key];
    return NSNotFound != index ? [self.values objectAtIndex:index] : nil;
}

- (NSUInteger)addStringValue:(NSString *)value
{
    NSUInteger key = self.hashIndex;
    
    if (NSNotFound != key)
    {
//        NSLog(@"Add entry to key: %ld",key);
        
        [self.values insertObject:[TRHashEntry entryWithKey:key value:value] atIndex:[self indexForAddValueWithKey:key]];
    }
    self.entriesCount++;
    
    return key;
}

- (void)removeStringValue:(NSString *)value forKey:(NSUInteger)key
{
    NSUInteger index = [self.values indexOfObjectPassingTest:^BOOL(TRHashEntry *obj, NSUInteger idx, BOOL *stop) {
        return obj.key == key;
    }];
    
    if (NSNotFound != index)
    {
        [self.values removeObjectAtIndex:index];
    }
}

- (NSArray *)valuesForKey:(NSUInteger)key
{
    return @[[self entryWithKey:key].value];
}

- (BOOL)isFreeValueForKey:(NSUInteger)key
{
    return nil == [self entryWithKey:key];
}

- (NSUInteger)hashIndex
{
    static NSInteger kHashIndexFunctionParameterA = 4;
    static NSInteger kHashIndexFunctionParameterB = 6;
    static NSInteger kHashIndexFunctionParameterC = 2;
    static NSInteger kHashIndexFunctionParameterH0 = 3;
    NSInteger i = self.entriesCount;
    NSUInteger hashIndex = NSNotFound;
    NSUInteger tries = self.entriesCount;
    while (NSNotFound == hashIndex && tries < self.size)
    {
        hashIndex = (kHashIndexFunctionParameterA * i * i + kHashIndexFunctionParameterB * i + kHashIndexFunctionParameterC + kHashIndexFunctionParameterH0) % self.size + tries;
        if (![self isFreeValueForKey:hashIndex])
        {
            hashIndex = NSNotFound;
            tries++;
        }
    }
    return hashIndex;
}

- (NSString *)description
{
    NSMutableString *description = [NSMutableString stringWithString:[super description]];
    [description appendString:@"\n"];
//    NSArray *sorted = [self.values sortedArrayUsingDescriptors:@[[NSSortDescriptor sortDescriptorWithKey:@"key" ascending:YES]]];
    for (TRHashEntry *entry in self.values)
    {
        [description appendFormat:@"key: %ld; value: %@\n",(long)entry.key,entry.value];
    }
    return description;
}

@end
