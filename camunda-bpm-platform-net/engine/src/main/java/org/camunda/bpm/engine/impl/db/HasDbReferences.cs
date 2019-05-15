﻿using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.db
{

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface HasDbReferences
	{

	  /// <returns> the ids of the entities that this entity references. Should
	  ///   only return ids for entities of the same type </returns>
	  ISet<string> ReferencedEntityIds {get;}

	  /// <returns> a map of the ids and the entities' classes that this
	  /// entity references. It's used when trying to determine if there
	  /// was an Optimistic Locking occurrence on an INSERT or UPDATE of
	  /// an object of this type. </returns>
	  IDictionary<string, Type> ReferencedEntitiesIdAndClass {get;}
	}

}