//
//  TRHashTable.h
//  HashTable
//
//  Created by Aleksey Bodnya on 5/16/14.
//  Copyright (c) 2014 Alexey Bodnya. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol TRHashTable <NSObject>

+ (instancetype)tableWithSize:(NSUInteger)size;

- (NSUInteger)addStringValue:(NSString *)value;
- (void)removeStringValue:(NSString *)value forKey:(NSUInteger)key;
- (NSArray *)valuesForKey:(NSUInteger)key;
- (NSUInteger)hashIndex;

@end
