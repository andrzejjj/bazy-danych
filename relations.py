#!/usr/bin/env python
# -*- coding: utf-8 -*-

import couchdb
from copy import deepcopy

map_fun = '''function(doc) {
					if (doc.tytul && doc.data_zalozenia)
						emit(doc.data_zalozenia, doc.tytul);
				}'''

map_fun2 = '''function(doc) {
					if (doc.tytul && doc.data_zalozenia)
						if (doc.posty.length > 0)
							for (var i in doc.posty)
								emit([doc.tytul, doc.posty[i].data_utworzenia], doc.posty[i].tresc);
				}'''
				
def readall():
	server = couchdb.Server()
	db = server['forum']
	
	results = db.query(map_fun2)
	
	print '----------------------------------------------------'
	for result in results:
		print result.key, ' ' , result.value
	print '----------------------------------------------------'	
	
def delete():
	server = couchdb.Server()
	db = server['forum']
	
	results = db.query(map_fun)
	for result in results['2014-03-02 11:27']:
		doc = db[result.id]
		
	for key in doc:
		if (type(doc[key]) is list):
			print key
			for x in doc[key]:
				print x
		else:
			print key, ' ', doc[key]	
			
	del doc['posty']		
	db.save(doc)
	print '\n'
	
	for key in doc:
		if (type(doc[key]) is list):
			print key
			for x in doc[key]:
				print x
		else:
			print key, ' ', doc[key]	
					
def init():
	server = couchdb.Server()
	db = server.create('forum')
	threads = []
	
	thread = { 'tytul' : 'Bazy Danych', 'data_zalozenia' : '2014-01-01 12:33'}
	posts = []
	posts.append({'tresc' : 'Pytanie o SQLa...', 'data_utworzenia' : '2014-02-02 7:15'})
	posts.append({'tresc' : 'Odpowiedz do SQLa...', 'data_utworzenia' : '2014-02-02 13:15'})
	thread['posty'] = posts
	threads.append(thread)
	
	thread = { 'tytul' : 'ASD', 'data_zalozenia' : '2014-03-02 11:27'}
	posts = []
	posts.append({'tresc' : 'Pytanie o BST...', 'data_utworzenia' : '2014-03-05 6:11'})
	posts.append({'tresc' : 'Implementacja BST...', 'data_utworzenia' : '2014-03-05 12:20'})
	posts.append({'tresc' : 'Poprawienie implementacji BST...', 'data_utworzenia' : '2014-03-06 23:59'})
	thread['posty'] = posts
	threads.append(thread)
	
	for thread in threads:
		db.save(thread)
	
def clean():
	server = couchdb.Server()
	del server['forum']
	
def main():
	
	try:
		init()
		readall()
		delete()
		
		readall()
		
	except Exception as e:
		print e
		
	finally:
		clean()
		
		
	return 0

if __name__ == '__main__':
	main()

