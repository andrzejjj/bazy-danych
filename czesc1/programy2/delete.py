#!/usr/bin/env python
# -*- coding: utf-8 -*-

import couchdb
from copy import deepcopy

map_fun = '''function(doc) {
					if (doc.tytul && doc.data_zalozenia)
						emit(doc.data_zalozenia, doc.tytul);
				}'''
				
def readall():
	server = couchdb.Server()
	db = server['forum']
	
	results = db.query(map_fun)
	
	print '----------------------------------------------------'
	for result in results:
		print result.key + '     ' + result.value
	print '----------------------------------------------------'	
	
def delete():
	server = couchdb.Server()
	db = server['forum']
	
	results = db.query(map_fun)
	for result in results['2014-01-01 12:33']:
		doc = db[result.id]
		
	for key in doc:
		print key + ' ' + doc[key]	
		
	db.delete(doc)
		
def init():
	server = couchdb.Server()
	db = server.create('forum')
	threads = []
	threads.append({ 'tytul' : 'Bazy Danych', 'data_zalozenia' : '2014-01-01 12:33'})
	threads.append({ 'tytul' : 'Algorytmy i Struktury Danych', 'data_zalozenia' : '2014-03-02 11:27'})
	threads.append({ 'tytul' : 'Kryptografia', 'data_zalozenia' : '2014-04-23 9:58'})
	threads.append({ 'tytul' : 'Teoria Wspolbieznosci', 'data_zalozenia' : '2013-11-11 21:03'})
	
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

