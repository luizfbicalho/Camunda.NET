using System;

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
	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using GetNextIdBlockCmd = org.camunda.bpm.engine.impl.cmd.GetNextIdBlockCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DbIdGenerator : IdGenerator
	{

	  protected internal int idBlockSize;
	  protected internal long nextId;
	  protected internal long lastId;

	  protected internal CommandExecutor commandExecutor;

	  public DbIdGenerator()
	  {
		reset();
	  }

	  public virtual string NextId
	  {
		  get
		  {
			  lock (this)
			  {
				if (lastId < nextId)
				{
				  NewBlock;
				}
				long _nextId = nextId++;
				return Convert.ToString(_nextId);
			  }
		  }
	  }

	  protected internal virtual void getNewBlock()
	  {
		  lock (this)
		  {
			// TODO http://jira.codehaus.org/browse/ACT-45 use a separate 'requiresNew' command executor
			IdBlock idBlock = commandExecutor.execute(new GetNextIdBlockCmd(idBlockSize));
			this.nextId = idBlock.NextId;
			this.lastId = idBlock.LastId;
		  }
	  }

	  public virtual int IdBlockSize
	  {
		  get
		  {
			return idBlockSize;
		  }
		  set
		  {
			this.idBlockSize = value;
		  }
	  }


	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
		  set
		  {
			this.commandExecutor = value;
		  }
	  }


	  /// <summary>
	  /// Reset inner state so that the generator fetches a new block of IDs from the database
	  /// when the next ID generation request is received.
	  /// </summary>
	  public virtual void reset()
	  {
		nextId = 0;
		lastId = -1;
	  }
	}

}